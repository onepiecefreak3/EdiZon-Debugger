using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJine.Lua;
using System.IO;

namespace EdiZonDebugger
{
    public static class Lua
    {
        public static bool InitializeScript(ref LuaContext luaContext, string luaFile, string saveFile, out string message)
        {
            message = String.Empty;

            try
            {
                luaContext = new LuaContext();
                luaContext.reg("edizon.getSaveFileBuffer", getSaveFileBuffer(saveFile));
                luaContext.reg("edizon.getSaveFileString", getSaveFileString(saveFile));
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

        //public static void RegisterSaveBuffer(ref LuaContext luaContext, string saveFile)
        //{
        //    luaContext.reg("edizon.getSaveFileBuffer", getSaveFileBuffer(saveFile));
        //}

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

        public static byte[] GetModifiedSaveBuffer(LuaContext luaContext)
        {
            var res = luaContext.exec("getModifiedSaveFile");

            return (byte[])res.First();
        }
        #endregion

        #region Delegates
        private static Func<byte[]> getSaveFileBuffer(string saveFile)
        {
            return new Func<byte[]>(() =>
            {
                return File.ReadAllBytes(saveFile);
            });
        }

        private static Func<string> getSaveFileString(string saveFile)
        {
            return new Func<string>(() =>
            {
                return File.ReadAllText(saveFile);
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
