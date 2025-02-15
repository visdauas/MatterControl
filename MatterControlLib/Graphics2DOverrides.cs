﻿/*
Copyright (c) 2018, Lars Brubaker
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

using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	[Flags]
	public enum LineArrows
	{
		None = 0,
		Start = 1,
		End = 2,
		Both = 3
	}


	public static class Graphics2DOverrides
	{
		public static void Ring(this Graphics2D graphics2D, Vector2 center, double radius, double width, Color color)
		{
			var ring = new Ellipse(center, radius);
			var ringStroke = new Stroke(ring, width);
			graphics2D.Render(ringStroke, color);
		}

		public static void DrawMeasureLine(this Graphics2D graphics2D, Vector2 lineStart, Vector2 lineEnd, LineArrows arrows, ThemeConfig theme)
		{
			graphics2D.Line(lineStart, lineEnd, theme.TextColor);

			Vector2 direction = lineEnd - lineStart;
			if (direction.LengthSquared > 0
				&& (arrows.HasFlag(LineArrows.Start) || arrows.HasFlag(LineArrows.End)))
			{
				var arrow = new VertexStorage();
				arrow.MoveTo(-3, -5);
				arrow.LineTo(0, 0);
				arrow.LineTo(3, -5);

				if (arrows.HasFlag(LineArrows.End))
				{
					double rotation = Math.Atan2(direction.Y, direction.X);
					IVertexSource correctRotation = new VertexSourceApplyTransform(arrow, Affine.NewRotation(rotation - MathHelper.Tau / 4));
					IVertexSource inPosition = new VertexSourceApplyTransform(correctRotation, Affine.NewTranslation(lineEnd));
					graphics2D.Render(inPosition, theme.TextColor);
				}

				if (arrows.HasFlag(LineArrows.Start))
				{
					double rotation = Math.Atan2(direction.Y, direction.X) + MathHelper.Tau / 2;
					IVertexSource correctRotation = new VertexSourceApplyTransform(arrow, Affine.NewRotation(rotation - MathHelper.Tau / 4));
					IVertexSource inPosition = new VertexSourceApplyTransform(correctRotation, Affine.NewTranslation(lineStart));
					graphics2D.Render(inPosition, theme.TextColor);
				}
			}
		}
	}
}