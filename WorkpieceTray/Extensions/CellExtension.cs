using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkpieceTray.Extensions
{
    public static class CellExtension
    {
        public static (int index, string cellName,int currentRow,int currentCol) ToIndexAndName(this int coloums, int currentRow, int currentCol)
        {
            var rowFlag = ASCIIEncoding.UTF8.GetString(new byte[] { (byte)(currentRow + 64) });
            var index = coloums * (currentRow - 1) + currentCol;
            var name = $"{rowFlag}{currentCol}";
            return (index, name, currentRow, currentCol);
        }
    }
}
