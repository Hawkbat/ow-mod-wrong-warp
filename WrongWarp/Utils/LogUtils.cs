using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Utils
{
    public static class LogUtils
    {
        public static void Log(string message) => WrongWarpMod.Instance.ModHelper.Console.WriteLine(message, MessageType.Info);
        public static void Warn(string message) => WrongWarpMod.Instance.ModHelper.Console.WriteLine(message, MessageType.Warning);
        public static void Success(string message) => WrongWarpMod.Instance.ModHelper.Console.WriteLine(message, MessageType.Success);
        public static void Error(string message) => WrongWarpMod.Instance.ModHelper.Console.WriteLine(message, MessageType.Error);
        public static void Exception(Exception ex) => WrongWarpMod.Instance.ModHelper.Console.WriteLine(ex.ToString(), MessageType.Error);
    }
}
