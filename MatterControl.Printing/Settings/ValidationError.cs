﻿/*
Copyright (c) 2019, John Lewin
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

using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public enum ValidationErrorLevel
	{
		Information,
		Warning,
		Error
	}

	public class ValidationError
	{
		public ValidationError(string id)
		{
			this.ID = id;
		}

		public string ID { get; }

		public string Error { get; set; }

		public string Details { get; set; }

		public ValidationErrorLevel ErrorLevel { get; set; } = ValidationErrorLevel.Error;

		public NamedAction FixAction { get; set; }
	}

	public static class ValidationErrors
	{
		public static readonly string BedLevelingTemperature = nameof(BedLevelingTemperature);
		public static readonly string BedLevelingMesh = nameof(BedLevelingMesh);
		public static readonly string ExceptionDuringSliceSettingsValidation = nameof(ExceptionDuringSliceSettingsValidation);
		public static readonly string ItemCannotBeExported = nameof(ItemCannotBeExported);
		public static readonly string ItemToAMFExportInvalid = nameof(ItemToAMFExportInvalid);
		public static readonly string ItemToSTLExportInvalid = nameof(ItemToSTLExportInvalid);
		public static readonly string NoItemsToExport = nameof(NoItemsToExport);
		public static readonly string NoPrintableParts = nameof(NoPrintableParts);
		public static readonly string NoZipItemsToExport = nameof(NoZipItemsToExport);
		public static readonly string PrinterDisconnected = nameof(PrinterDisconnected);
		public static readonly string PrinterSetupRequired = nameof(PrinterSetupRequired);
		public static readonly string SettingsUpdateAvailable = nameof(SettingsUpdateAvailable);
		public static readonly string UnsupportedParts = nameof(UnsupportedParts);
		public static readonly string ZOffset = nameof(ZOffset);
	}
}