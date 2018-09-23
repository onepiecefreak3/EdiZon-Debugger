using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using EdiZonDebugger.Helper;

using LuaWrapper;

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

                luaContext.RegisterFunction("edizon", "getSaveFileBuffer", getSaveFileBuffer(saveFile));
                luaContext.RegisterFunction("edizon", "getSaveFileString", getSaveFileString(saveFile, encoding));
                luaContext.RegisterFunction("", "print", Print());

                luaContext.LoadFromFile(luaFile);
                luaContext.Execute();
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }

            return true;
        }

        public static object ExecuteCalculation(string calc, double input)
        {
            var lua = new LuaContext();
            lua.LoadFromString(
                $"function exec(value)" +
                $"return {calc}" +
                $"end"
                );

            lua.Execute();
            var res = lua.Execute("exec", input);

            return Convert.ToUInt32(res.result.First());
        }

        #region Script functions
        public static object GetValueFromSaveFile(LuaContext luaContext, string[] strArgs, int[] intArgs)
        {
            luaContext.RegisterFunction("edizon", "getStrArgs", getStrArgs(strArgs));
            luaContext.RegisterFunction("edizon", "getIntArgs", getIntArgs(intArgs));

            var res = luaContext.Execute("getValueFromSaveFile");

            if (!res.success)
            {
                LogConsole.Instance.Log(res.result.Where(r => r != null).Aggregate("", (a, b) => a + b.ToString() + Environment.NewLine), LogLevel.LUA);
                return null;
            }

            return res.result.First();
        }

        public static void SetValueInSaveFile(LuaContext luaContext, string[] strArgs, int[] intArgs, object value)
        {
            luaContext.RegisterFunction("edizon", "getStrArgs", getStrArgs(strArgs));
            luaContext.RegisterFunction("edizon", "getIntArgs", getIntArgs(intArgs));

            luaContext.Execute("setValueInSaveFile", value);
        }

        public static byte[] GetModifiedSaveBuffer(LuaContext luaContext)
        {
            var res = luaContext.Execute("getModifiedSaveFile");
            var firstRes = res.result.First();

            if (firstRes is object[] save)
                return save.Select(x => Convert.ToByte(x)).ToArray();

            return new byte[0];
        }
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
