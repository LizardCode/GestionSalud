﻿using System.Collections.Generic;

namespace LizardCode.Framework.Helpers.BarcodeLib
{
    /// <summary>
    ///  Barcode interface for symbology layout.
    ///  Written by: Brad Barnhill
    /// </summary>
    interface IBarcode
    {
        string Encoded_Value
        {
            get;
        }//Encoded_Value

        string RawData
        {
            get;
        }//Raw_Data

        List<string> Errors
        {
            get;
        }//Errors

    }//interface
}//namespace
