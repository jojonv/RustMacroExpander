using RustMacroExpander.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RustMacroExpander.Helpers
{
    internal static class StateManager
    {
        struct SerializationAwaiter<T>
        {
            public string Path { get; private set; }
            public T Obj { get; private set; }

            public SerializationAwaiter(string p, T o)
            {
                Path = p;
                Obj = o;
            }
        }

        readonly static string settingsPath = "Settings";
        readonly static string rustBridgePath = "RustBridge";
        readonly static string extension = ".bin";
        readonly static AppDomain currentDomain;

        static Queue<SerializationAwaiter<Object>> serializationQueue = new Queue<SerializationAwaiter<object>>();


        static StateManager()
        {
            (currentDomain = AppDomain.CurrentDomain).UnhandledException += (s, a) =>
            {

                var e = (a.ExceptionObject as Exception)?.InnerException as SettingsException;
                if (e == null)
                    return;

                // Settings're wrong, delete serialized file, and let restore settings at the next start
                string path = GetPath<Settings>();
                File.Delete(path);
            };

            App.Current.Exit += (s, a) =>
            {
                while (serializationQueue.Count > 0)
                {
                    var awaiter = serializationQueue.Dequeue();
                    Serialize(awaiter.Obj, awaiter.Path);
                }
            };
        }

        private static void StateManager_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void Serialize(object o, string fileName)
        {
            // TODO handle exceptions
            //
            using (var memStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(o.GetType());
                serializer.WriteObject(memStream, o);

                using (var fStream = new FileStream(fileName
                                                    , FileMode.Create
                                                    , FileAccess.Write
                                                    , FileShare.Read))
                {
                    memStream.Position = 0;
                    memStream.CopyTo(fStream);
                }
            }
        }

        static string GetPath<T>()
        {
            if (typeof(T) == typeof(Settings))
            {
                return $"{settingsPath}{extension}";
            }
            else if (typeof(T) == typeof(RustBridge))
            {
                return $"{rustBridgePath}{extension}";
            }
            else
                throw new ArgumentException($"Can't serialize object of type {typeof(T).ToString()}");
        }

        public static void Save<T>(T o)
        {
            if (o == null)
                throw new ArgumentNullException("Can't serialize null");

            string path = GetPath<T>();
            serializationQueue.Enqueue(new SerializationAwaiter<object>(path, o));
        }

        public static T Load<T>()
        {
            string path = GetPath<T>();

            try
            {
                using (var stream = new FileStream(path
                                                    , FileMode.Open
                                                    , FileAccess.Read
                                                    , FileShare.Read))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    var obj = serializer.ReadObject(stream);
                    return (T)obj;
                }
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
            catch (SerializationException)
            {
                return default(T);
            }
        }
    }
}
