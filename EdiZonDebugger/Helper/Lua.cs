using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJine.Lua;
using System.IO;

using EdiZonDebugger.Helper;

namespace EdiZonDebugger
{
    public static class Lua
    {
        public static bool InitializeScript(ref LuaContext luaContext, string luaFile, string saveFile, string encoding, out string message)
        {
            message = String.Empty;

            try
            {
                luaContext = new LuaContext();
                luaContext.reg("edizon.getSaveFileBuffer", getSaveFileBuffer(saveFile));
                luaContext.reg("edizon.getSaveFileString", getSaveFileString(saveFile, encoding));
                luaContext.reg("print", new Action<string>((string s) =>
                {
                    LogConsole.Instance.Log(s, LogLevel.LUA);
                }));
                luaContext.load(luaFile);
            }
            catch (Exception e)
            {
                if (e is FormatException)
                    message = "vJine.Lua is a 32bit only library but the app expects 64bit.";
                else
                    message = e.Message;

                return false;
            }

            return true;
        }

        public static object ExecuteCalculation(string calc, long input)
        {
            var lua = new LuaContext();
            lua.set("value", input);
            var res = lua.inject("return " + calc);

            return Convert.ToUInt32(res.First());
        }

        #region Script functions
        public static object GetValueFromSaveFile(LuaContext luaContext, string[] strArgs, int[] intArgs)
        {
            luaContext.reg("edizon.getStrArgs", getStrArgs(strArgs));
            luaContext.reg("edizon.getIntArgs", getIntArgs(intArgs));

            var res = luaContext.exec("getValueFromSaveFile");

            return res.First();
        }

        public static void SetValueInSaveFile(LuaContext luaContext, string[] strArgs, int[] intArgs, object value)
        {
            luaContext.reg("edizon.getStrArgs", getStrArgs(strArgs));
            luaContext.reg("edizon.getIntArgs", getIntArgs(intArgs));

            luaContext.exec("setValueInSaveFile", value);
        }

        //public static byte[] GetModifiedSaveBuffer(LuaContext luaContext)
        //{
        //    var res = luaContext.exec("getModifiedSaveFile");

        //    return (byte[])res.First();
        //}
        #endregion

        #region Delegates
        private static Action<string> Print()
        {
            return new Action<string>((string s) =>
            {
                LogConsole.Instance.Log(s, LogLevel.LUA);
            });
        }

        private static Func<byte[]> getSaveFileBuffer(string saveFile)
        {
            return new Func<byte[]>(() =>
            {
                return File.ReadAllBytes(saveFile);
            });
        }

        private static Func<string> getSaveFileString(string saveFile, string encoding)
        {
            return new Func<string>(() =>
            {
                //ascii, utf-8, utf-16le and utf-16be
                switch (encoding)
                {
                    default:
                    case "ascii":
                        return File.ReadAllText(saveFile, Encoding.ASCII);
                    case "utf-8":
                        return File.ReadAllText(saveFile, Encoding.UTF8);
                    case "utf-16le":
                        return File.ReadAllText(saveFile, Encoding.Unicode);
                    case "utf-16be":
                        return File.ReadAllText(saveFile, Encoding.BigEndianUnicode);
                }
            });
        }

        private static Func<string[]> getStrArgs(string[] strArgs)
        {
            return new Func<string[]>(() =>
            {
                return strArgs;
            });
        }

        private static Func<int[]> getIntArgs(int[] intArgs)
        {
            return new Func<int[]>(() =>
            {
                return intArgs;
            });
        }
        #endregion
    }
}
