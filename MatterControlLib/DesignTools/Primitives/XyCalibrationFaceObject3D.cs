﻿/*
Copyright (c) 2018, Lars Brubaker, John Lewin
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System.ComponentModel;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.VertexSource;
using MatterHackers.DataConverters3D;
using MatterHackers.Localizations;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.DesignTools
{
	public class XyCalibrationFaceObject3D : Object3D
	{
		public double NozzleWidth { get; set; } = .4;

		public XyCalibrationFaceObject3D()
		{
			Name = "Calibration Faces".Localize();
		}

		public double BaseHeight { get; set; } = .4;

		[DisplayName("Material")]
		public int CalibrationMaterialIndex { get; set; } = 1;

		public override bool CanFlatten => true;

		public double ChangingHeight { get; set; } = .4;

		public int Layers { get; set; } = 10;

		public double Offset { get; set; } = .5;

		public double WipeTowerSize { get; set; } = 10;

		private double TabDepth => NozzleWidth * tabScale * 5;

		private double tabScale = 3;

		private double TabWidth => NozzleWidth * tabScale * 3;

		public static async Task<XyCalibrationFaceObject3D> Create(int calibrationMaterialIndex = 1,
							double baseHeight = .25,
							double changingHeight = .2,
							double offset = .5,
							double nozzleWidth = .4,
							double wipeTowerSize = 15,
							int layers = 6)
		{
			var item = new XyCalibrationFaceObject3D()
			{
				WipeTowerSize = wipeTowerSize,
				Layers = layers,
				CalibrationMaterialIndex = calibrationMaterialIndex,
				BaseHeight = baseHeight,
				ChangingHeight = changingHeight,
				Offset = offset,
				NozzleWidth = nozzleWidth
			};

			await item.Rebuild();
			return item;
		}

		public override async void OnInvalidate(InvalidateArgs invalidateType)
		{
			if (invalidateType.InvalidateType.HasFlag(InvalidateType.Properties)
				&& invalidateType.Source == this)
			{
				await Rebuild();
			}
			else
			{
				base.OnInvalidate(invalidateType);
			}
		}

		public override Task Rebuild()
		{
			this.DebugDepth("Rebuild");

			tabScale = 3;

			// by default we don't want tab with to be greater than 10 mm
			if (TabWidth > 10)
			{
				tabScale = 1;
			}
			else if (TabWidth > 5)
			{
				tabScale = 2;
			}

			using (RebuildLock())
			{
				using (new CenterAndHeightMaintainer(this))
				{
					this.Children.Modify((list) =>
					{
						list.Clear();
					});

					var calibrateX = GetTab(true);
					this.Children.Add(calibrateX);
					var calibrateY = GetTab(false);
					this.Children.Add(calibrateY);
					// add in the corner connector
					this.Children.Add(new Object3D()
					{
						Mesh = PlatonicSolids.CreateCube(),
						Matrix = Matrix4X4.CreateTranslation(-1 / 2.0, 1 / 2.0, 1 / 2.0) * Matrix4X4.CreateScale(TabDepth, TabDepth, BaseHeight),
						Color = Color.LightBlue
					});

					if (WipeTowerSize > 0)
					{
						// add in the wipe tower
						this.Children.Add(new Object3D()
						{
							Mesh = new CylinderObject3D(1, 1, 50).Mesh,
							Matrix = Matrix4X4.CreateTranslation(1 / 2.0, 1 / 2.0, 1 / 2.0)
								* Matrix4X4.CreateScale(WipeTowerSize, WipeTowerSize, BaseHeight + Layers * ChangingHeight)
								* Matrix4X4.CreateTranslation(TabDepth * 1, TabDepth * 2, 0),
							OutputType = PrintOutputTypes.WipeTower
						});
					}
				}
			}

			Parent?.Invalidate(new InvalidateArgs(this, InvalidateType.Mesh));
			return Task.CompletedTask;
		}

		private Object3D GetTab(bool calibrateX)
		{
			var content = new Object3D();

			var spaceBetween = NozzleWidth * tabScale;

			var shape = new VertexStorage();
			shape.MoveTo(0, 0);
			// left + spaces + blocks + right
			var sampleCount = 7;
			var baseWidth = (2 * spaceBetween) + ((sampleCount - 1) * spaceBetween) + (sampleCount * TabWidth) + (2 * spaceBetween);
			shape.LineTo(baseWidth, 0);
			if (calibrateX)
			{
				var origin = new Vector2(baseWidth, TabDepth / 2);
				var delta = new Vector2(0, -TabDepth / 2);
				var count = 15;
				for (int i = 0; i < count; i++)
				{
					delta.Rotate(MathHelper.Tau / 2 / count);
					shape.LineTo(origin + delta);
				}
			}
			else
			{
				shape.LineTo(baseWidth + TabDepth, TabDepth / 2); // a point on the left
			}
			shape.LineTo(baseWidth, TabDepth);
			shape.LineTo(0, TabDepth);

			content.Children.Add(new Object3D()
			{
				Mesh = shape.Extrude(BaseHeight),
				Color = Color.LightBlue
			});

			var position = new Vector2(TabWidth / 2 + 2 * spaceBetween, TabDepth / 2);
			var step = new Vector2(spaceBetween + TabWidth, 0);
			for (int i = 0; i < sampleCount; i++)
			{
				var offsetMultiple = i - 3;
				for (int j = 0; j < Layers; j++)
				{
					var calibrationMaterial = (j % 2 == 0);
					var cube = PlatonicSolids.CreateCube();
					var item = new Object3D()
					{
						Mesh = cube,
					};
					content.Children.Add(item);
					if (calibrationMaterial)
					{
						item.MaterialIndex = CalibrationMaterialIndex;
						item.Color = Color.Yellow;
						item.Matrix = Matrix4X4.CreateScale(TabWidth, TabDepth, ChangingHeight) * Matrix4X4.CreateTranslation(position.X, position.Y + Offset * offsetMultiple, BaseHeight + .5 * ChangingHeight + j * ChangingHeight);
					}
					else
					{
						item.Color = Color.LightBlue;
						item.Matrix = Matrix4X4.CreateScale(TabWidth + spaceBetween * 2, TabDepth, ChangingHeight) * Matrix4X4.CreateTranslation(position.X, position.Y, BaseHeight + .5 * ChangingHeight + j * ChangingHeight);
					}
				}
				position += step;
			}

			if (calibrateX)
			{
				content.Matrix = Matrix4X4.CreateRotationZ(MathHelper.Tau / 4) * Matrix4X4.CreateTranslation(0, TabDepth, 0);
			}

			return content;
		}
	}
}