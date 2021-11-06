﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace UAssetGUI
{
    public static class UAGUtils
    {
        internal static string _displayVersion = string.Empty;

        public static T TryGetElement<T>(this T[] array, int index)
        {
            if (array != null && index < array.Length)
            {
                return array[index];
            }
            return default(T);
        }

        public static object ArbitraryTryParse(this string input, Type type)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    return converter.ConvertFromString(input);
                }
            }
            catch (NotSupportedException) { }
            return null;
        }

        public static ContextMenuStrip MergeContextMenus(ContextMenuStrip one, ContextMenuStrip two)
        {
            if (one == null) return two;
            if (two == null) return one;

            one.Items.AddRange(two.Items);
            return one;
        }

        // ((Form1)x.DataGridView.Parent).nameMapContext;
        public static void UpdateContextMenuStripOfRow(DataGridViewRow x, ContextMenuStrip strip)
        {
            x.ContextMenuStrip = MergeContextMenus(x.ContextMenuStrip, strip);
            x.HeaderCell.ContextMenuStrip = MergeContextMenus(x.HeaderCell.ContextMenuStrip, strip);
            foreach (DataGridViewCell y in x.Cells)
            {
                y.ContextMenuStrip = MergeContextMenus(y.ContextMenuStrip, strip);
            }
        }

        public static void AdjustFormPosition(this Form frm1)
        {
            if (frm1.Owner != null) frm1.Location = new Point((frm1.Owner.Location.X + frm1.Owner.Width / 2) - (frm1.Width / 2), (frm1.Owner.Location.Y + frm1.Owner.Height / 2) - (frm1.Height / 2));
        }

        public static void UpdateObjectPropertyValues(UAsset asset, DataGridViewRow row, DataGridView dgv, ObjectPropertyData objData)
        {
            if (dgv == null || row == null || objData == null) return;

            bool underlineStyle = false;
            if (objData.Value.IsImport() && objData.Value == null)
            {
                row.Cells[3].Value = string.Empty;
            }
            else
            {
                row.Cells[3].Value = objData.Value.IsExport() ? "Jump" : (objData.Value.IsImport() ? objData.ToImport(asset)?.ObjectName?.ToString() : string.Empty);
                row.Cells[3].Tag = "CategoryJump";
                if (objData.Value.IsExport()) underlineStyle = true;
            }
            row.Cells[3].ReadOnly = objData.Value.IsImport();

            DataGridViewCellStyle sty = new DataGridViewCellStyle();
            if (underlineStyle)
            {
                Font styFont = new Font(dgv.Font.Name, dgv.Font.Size, FontStyle.Underline);
                sty.Font = styFont;
                sty.ForeColor = Color.Blue;
            }
            row.Cells[3].Style = sty;
        }

        public static T[] StripNullsFromArray<T>(this T[] usArr)
        {
            int c = 0;
            for (int num = 0; num < usArr.Length; num++)
            {
                if (usArr[num] != null) c++;
            }

            var newData = new T[c];
            int indexAdded = 0;
            for (int num = 0; num < usArr.Length; num++)
            {
                if (usArr[num] != null) newData[indexAdded++] = usArr[num];
            }
            return newData;
        }

        public static List<T> StripNullsFromList<T>(this List<T> usList)
        {
            for (int num = 0; num < usList.Count; num++)
            {
                if (usList[num] == null)
                {
                    usList.RemoveAt(num);
                    num--;
                }
            }
            return usList;
        }

        public static string ConvertByteArrayToString(this byte[] val)
        {
            if (val == null) return "";
            return BitConverter.ToString(val).Replace("-", " ");
        }

        public static byte[] ConvertStringToByteArray(this string val)
        {
            if (val == null) return new byte[0];
            string[] rawStringArr = val.Split(' ');
            byte[] byteArr = new byte[rawStringArr.Length];
            for (int i = 0; i < rawStringArr.Length; i++) byteArr[i] = Convert.ToByte(rawStringArr[i], 16);
            return byteArr;
        }

        /*
            UAssetGUI versions are formatted as follows: MAJOR.MINOR.BUILD.REVISION
            * MAJOR - incremented for very big changes or backwards-incompatible changes
            * MINOR - incremented for notable changes
            * BUILD - incremented for bug fixes or very small improvements
            * REVISION - incremented for test/alpha builds of the existing version
            
            2.0.0.0 > 1.5.0.0 > 1.4.1.0 > 1.4.0.1 > 1.4.0.0
        */
        public static bool IsUAGVersionLower(this Version v1)
        {
            Version fullUagVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return v1.CompareTo(fullUagVersion) > 0;
        }

        private static Control internalForm;
        public static void InitializeInvoke(Control control)
        {
            internalForm = control;
        }

        public static void InvokeUI(Action act)
        {
            if (internalForm.InvokeRequired)
            {
                internalForm.Invoke(act);
            }
            else
            {
                act();
            }
        }
    }
}
