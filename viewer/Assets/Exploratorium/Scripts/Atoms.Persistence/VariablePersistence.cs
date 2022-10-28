using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector2Int = UnityEngine.Vector2Int;
using Vector3 = UnityEngine.Vector3;
using Vector3Int = UnityEngine.Vector3Int;
using Vector4 = UnityEngine.Vector4;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Object = UnityEngine.Object;
using UnityEngine;
using UnityEngine.Serialization;

namespace Atoms.Persistence
{
    public class VariablePersistence : MonoBehaviour
    {
        [SerializeField]
        [ValidateInput(
            nameof(IsPossiblyValidFilename),
            "A valid file name must be specified!",
            InfoMessageType.Error
        )]
        private StringReference filename = new StringReference(DefaultFilename);

        [SerializeField] private StringVariable profile;
        [SerializeField] private StringValueList profileNames;

        [ShowInInspector] [Sirenix.OdinInspector.ReadOnly]
        private const string DefaultFilename = nameof(VariablePersistence);

        [ShowInInspector] [Sirenix.OdinInspector.ReadOnly]
        public const string DefaultProfile = "Default";

        [SerializeField]
        [Tooltip(
            "The path that any relative path given as filename is relative to. Absolute paths will always be used as is.")]
        private FileLocation location = FileLocation.PersistentDataPath;

        [ShowInInspector]
        public string Profile
        {
            get
            {
                if (profile == null || string.IsNullOrEmpty(profile.Value))
                    return DefaultProfile;
                return profile.Value;
            }
        }

        [ShowInInspector]
        public string[] ProfileNames
        {
            get
            {
                if (profileNames == null || profileNames.Count == 0)
                    return new[] {DefaultProfile};
                return profileNames.ToArray();
            }
        }

        public string FilePath
        {
            get
            {
                var filePath = SanitizeFilename(filename.Value);
                if (!IsPossiblyValidFilename(filename))
                    filePath = DefaultFilename;

                if (Path.IsPathRooted(filePath))
                {
                    var dir = Path.GetDirectoryName(filePath) ?? Application.persistentDataPath;
                    var file = Path.GetFileNameWithoutExtension(filePath);
                    return Path
                        .Combine(dir, $"{file}{(Application.isEditor ? "_EDITOR" : "")}.json")
                        .Replace(@"\", "/");
                }
                else
                {
                    filePath = location switch
                    {
                        FileLocation.DataPath => Path.Combine(Application.dataPath, filePath),
                        FileLocation.PersistentDataPath => Path.Combine(Application.persistentDataPath, filePath),
                        FileLocation.StreamingAssetsPath => Path.Combine(Application.streamingAssetsPath, filePath),
                        FileLocation.AbsolutePath => filePath,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    var containingDirectory = Path.GetDirectoryName(filePath);
                    var file = Path.GetFileNameWithoutExtension(filePath);
                    if (!string.IsNullOrEmpty(containingDirectory))
                        return Path
                            .Combine(Application.persistentDataPath, containingDirectory,
                                $"{file}{(Application.isEditor ? "_EDITOR" : "")}.json")
                            .Replace(@"\", "/");
                    else
                        return Path
                            .Combine(Application.persistentDataPath,
                                $"{file}{(Application.isEditor ? "_EDITOR" : "")}.json")
                            .Replace(@"\", "/");
                }
            }
        }

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            Converters =
            {
                new ReadbackTypeConverter<Vector2>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Vector3>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Vector4>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Plane>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Quaternion>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Vector2Int>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Vector3Int>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Rect>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Bounds>(value => value.GetPrimitive()),
                new ReadbackTypeConverter<Matrix4x4>(value => value.GetPrimitive()),
            },
            Formatting = Formatting.Indented,
            Culture = CultureInfo.InvariantCulture
        };

        private static string SanitizeFilename(string path)
        {
            return path.TrimNonAscii();
        }

        [Tooltip(
            "Save variable values when this event is invoked."
        )]
        [SerializeField]
        private VoidEvent saveNow;

        [Tooltip(
            "Load variable values when this event is invoked."
        )]
        [SerializeField]
        private VoidEvent loadNow;

        [SerializeField] private StringVariable statusVariable;

        [Tooltip(
            "Save variable values when the component is destroyed. For example on application exit. Useful when variables are changed during runtime."
        )]
        [SerializeField]
        private bool saveOnDestroy = true;

        [Tooltip(
            "Load variables when the component is loaded. Useful to allow offline edits in the persistent data file to get applied when the application starts. I.e. to implement initialization parameters."
        )]
        [SerializeField]
        private bool loadOnAwake = true;

        [Tooltip(
            "Load variables when the filename has changed. Useful to implement specifying settings filename with commandline arguments."
        )]
        [SerializeField]
        private bool loadOnFileChanged = true;

        [Tooltip(
            "Load variables when the profile has changed. Useful to implement specifying settings filename with commandline arguments."
        )]
        [SerializeField]
        private bool loadOnProfileChanged = true;

        [Tooltip("Keep the current version of the persistent data whenever a new one is about to be created.")]
        [SerializeField]
        private bool keepVersion = true;

        [Tooltip(
            "Whether to create a new data file with default values when the application version, noted in the existing file metadata, does not match the current application version."
        )]
        [SerializeField]
        private bool revertToDefaults = true;

        /*[InfoBox(
            "All stream variables referenced here are serialized into a .json file in the application's persistent data path and " +
            "loaded back, first thing when the application starts.")]*/
        [FormerlySerializedAs("_variables")]
        [Tooltip("The variables that should have their values retained across sessions.")]
        [SerializeField]
        private List<AtomBaseVariable> variables = new List<AtomBaseVariable>();

        [SerializeField] private VoidEvent saved;
        [SerializeField] private VoidEvent applied;
        [SerializeField] private StringEvent error;

        public event Action Saved;
        public event Action Applied;
        public event Action<string> Error;

        // private string _json;
        //private Schema _schema;


#if UNITY_EDITOR
        [Button("Reveal in Finder")]
        private void RevealInFinder()
        {
            Debug.Log(FilePath);
            UnityEditor.EditorUtility.RevealInFinder(FilePath);
        }
#endif

        // parameterless alias to the standard load routine
        [Button("Load Profile Variables")]
        public void LoadAndApplyNow()
        {
            LoadAndApplyNow(Profile);
        }

        public void LoadAndApplyNow(string profileToApply)
        {
            Dictionary<string, ProfileSchema> loadedProfiles = LoadProfiles();
            if (loadedProfiles.ContainsKey(profileToApply))
            {
                Debug.Log($"[{nameof(VariablePersistence)}] {name} : Applying schema for profile {Profile}.", this);
                ApplySchema(loadedProfiles[profileToApply], ref variables);
            }
            else
            {
                Debug.LogError(
                    $"[{nameof(VariablePersistence)}] {name} : Profile '{profile.Value}' not found in '{FilePath}'.",
                    this
                );
            }
        }

        // parameterless alias to the standard load routine
        [Button("Save Current Profile Variables")]
        public void SaveCurrentProfile() => SaveCurrentProfile(keepVersion);

        private void Awake()
        {
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            if (loadOnFileChanged && filename != null && filename.Usage == AtomReferenceUsage.VARIABLE)
                filename.GetEvent<StringEvent>().Register(LoadAndApplyNow);

            if (loadOnProfileChanged && profile.Changed != null)
                profile.GetEvent<StringEvent>().Register(LoadAndApplyNow);

            // allow saving/loading via events
            if (saveNow != null)
                saveNow.Register(SaveCurrentProfile);

            if (loadNow != null)
                loadNow.Register(LoadAndApplyNow);

            // try loading existing data if it exists or initialize data if not.
            if (loadOnAwake)
            {
                
                Dictionary<string, ProfileSchema> profileDict = LoadProfiles();
                if (profileDict == null || !profileDict.ContainsKey(Profile) ||
                    (!profileDict[Profile].Meta.Version.Equals(Application.version) && revertToDefaults))
                {
                    Debug.LogWarning(
                        $"[{nameof(VariablePersistence)}] {name} : Version mismatch in {FilePath}. Saving defaults."
                    );
                    SaveCurrentProfile(keepVersion);
                }
                else
                {
                    Debug.Log($"[{nameof(VariablePersistence)}] {name} : Applying schema for profile {Profile}.");
                    ApplySchema(profileDict[Profile], ref variables);
                }
            }
        }

        private Dictionary<string, ProfileSchema> LoadProfiles()
        {
            //Dictionary<string, ProfileSchema> loadedProfiles;
            try
            {
                if (!_serializerSettings.Converters.Any(i => i is VariableListConverter))
                    _serializerSettings.Converters.Add(new VariableListConverter(variables));

                Debug.Assert(!string.IsNullOrEmpty(FilePath));

                string json = File.ReadAllText(FilePath);

                var profiles =
                    JsonConvert.DeserializeObject<Dictionary<string, ProfileSchema>>(json, _serializerSettings);
                if (profileNames != null && profiles?.Count > 0)
                {
                    profileNames.Clear();
                    profileNames.AddRange(profiles.Keys);
                }

                Debug.Log(
                    $"[{nameof(VariablePersistence)}] {name} : {profiles?.Count} profiles with {profiles?.Sum(it => it.Value.Variables.Count)} total variables <b>loaded</b> from {FilePath}.\n{json}",
                    this);
                return profiles;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.Log($"[{nameof(VariablePersistence)}] {name} : Could not load '{FilePath}'. Aborted.", this);
                // assume there was a serialization error
                Error?.Invoke(e.Message);
                if (error != null)
                    error.Raise(e.Message);
                return null;
            }
        }

        private void SaveCurrentProfile(bool keepVer = true)
        {
            var json = SerializeVariablesToJson();

            if (WriteFile(json, keepVer))
            {
                Debug.Log(
                    $"[{nameof(VariablePersistence)}] {name} : {variables.Count} variables <b>saved</b> to '{FilePath}'.",
                    this);
                Saved?.Invoke();
                if (saved != null)
                    saved.Raise();
            }
            else
            {
                Debug.Log($"[{nameof(VariablePersistence)}] {name} : Failed to save variables to '{FilePath}'.", this);
            }
        }

        private void ApplySchema(ProfileSchema profileSchema, ref List<AtomBaseVariable> vars)
        {
            foreach (ProfileSchema.Variable variable in profileSchema.Variables)
            {
                AtomBaseVariable atomVariable = vars.FirstOrDefault(it => it != null && it.Id == variable.Fingerprint);

                if (atomVariable == null)
                {
                    Debug.Log($"[{nameof(VariablePersistence)}] : No match for '{variable.Fingerprint}'", this);
                    continue;
                }

                if (atomVariable.IsPrimitive() || atomVariable.IsValueType())
                {
                    // assign simple types (for which there is a built-in or custom serializer directly
                    // match.Type.IsPrimitive matches Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single
                    // match.Type.IsValueType generally expects unity math types like Vector3, Quaternion, ...
                    atomVariable.BaseValue = Convert.ChangeType(variable.Value, atomVariable.BaseValue.GetType());
                    Debug.Log(
                        $"[{nameof(VariablePersistence)}] : {atomVariable.name}<{atomVariable.GetType().Name}> = {atomVariable.BaseValue}",
                        this);
                }
                else if (atomVariable.BaseValue is string)
                {
                    // assign string values directly
                    atomVariable.BaseValue = Convert.ChangeType(variable.Value, atomVariable.BaseValue.GetType());
                    var redact = atomVariable.name.Contains("assword") || atomVariable.name.Contains("ecret");
                    var value = atomVariable.BaseValue as string;
                    Debug.Log(
                        $"[{nameof(VariablePersistence)}] : {atomVariable.name}<{atomVariable.GetType().Name}> = '{(redact ? Redact(value) : value)}'",
                        this);
                }
                else
                {
                    // unpack objects and assign their properties individually (this is not really supported and works only on very simple objects composed of primitive types)
                    if (variable.Value is JToken jToken)
                    {
                        AssignToObject(jToken, atomVariable.BaseValue, atomVariable.GetType());
                        Debug.Log(
                            $"[{nameof(VariablePersistence)}] : {atomVariable.name}<{atomVariable.GetType().Name}> = '{atomVariable.BaseValue}'",
                            this);
                    }
                    else
                        Debug.LogError(
                            $"[{nameof(VariablePersistence)}] : Value is not of the expected type. Expected {nameof(JToken)}, got {variable.Value.GetType().Name} instead.",
                            this
                        );
                }
            }

            Debug.Log(
                $"[{nameof(VariablePersistence)}] : {profileSchema.Variables.Count} variables <b>applied</b>.",
                this);

            Applied?.Invoke();
            if (applied != null)
                applied.Raise();
        }

        private static bool IsPossiblyValidFilename(StringReference name)
        {
            if (name.IsUnassigned)
                return false;

            FileInfo fi;
            try
            {
                fi = new FileInfo(name.Value);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            return fi != null;
        }


        private bool WriteFile(string contents, bool keepVer)
        {
            try
            {
                var current = new FileInfo(FilePath);
                if (keepVersion && current.Exists)
                {
                    var versionPath = current.DirectoryName +
                                      Path.DirectorySeparatorChar +
                                      current.Name +
                                      "." +
                                      DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffK");
                    current.MoveTo(versionPath);
                    Debug.Log(
                        $"[{nameof(VariablePersistence)}] : Current version of '{FilePath}' moved to '{versionPath}'.",
                        this
                    );

                    // prune old versions
                    FileInfo nextFile = new FileInfo(FilePath);
                    DirectoryInfo nextFileDirectory = nextFile.Directory;
                    if (nextFileDirectory != null)
                    {
                        var fileVersions = nextFileDirectory
                            .GetFiles($"*{nextFile.Name}*")
                            .OrderByDescending(i => i.CreationTimeUtc)
                            .Skip(2)
                            //.Where(i => i.CreationTimeUtc < (DateTime.UtcNow - TimeSpan.FromDays(1)))
                            //.Take(10)
                            .ToList();
                        foreach (FileInfo file in fileVersions)
                            file.Delete();
                        Debug.Log(
                            $"[{nameof(VariablePersistence)}] : {fileVersions.Count} old versions removed.",
                            this);
                    }
                }

                File.WriteAllText(FilePath, contents);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Error?.Invoke(e.Message);
                if (error != null)
                    error.Raise(e.Message);
                return false;
            }

            return true;
        }

        private string SerializeVariablesToJson()
        {
            // load existing profiles
            Dictionary<string, ProfileSchema>
                loadedProfiles = LoadProfiles() ?? new Dictionary<string, ProfileSchema>();

            var descriptionFi = typeof(BaseAtom).GetField("_developerDescription", BindingFlags.NonPublic);
            loadedProfiles[Profile] = new ProfileSchema
            {
                Meta = new ProfileSchema.Context
                {
                    Utc = DateTime.UtcNow,
                    CompanyName = Application.companyName,
                    ProductName = Application.productName,
                    Version = Application.version,
                    BuildGUID = Application.buildGUID,
                    Identifier = Application.identifier,
                    Platform = Application.platform.ToString(),
                    Genuine = Application.genuine,
                    UnityVersion = Application.unityVersion,
                    SystemLanguage = Application.systemLanguage.ToString()
                },
                Variables = variables.Where(it => it != null)
                    .Select(
                        it =>
                        {
                            var t = it.BaseValue.GetType();
                            ;
                            return new ProfileSchema.Variable
                            {
                                Name = it.name,
                                Fingerprint = it.Id,
                                Value = it.BaseValue,
                                Type = t.GetNiceName(),
                                Description = (descriptionFi?.GetValue(it) + EnumValueHints(t)).Trim()
                            };
                        })
                    .ToList()
            };
            return JsonConvert.SerializeObject(loadedProfiles, _serializerSettings);

            string EnumValueHints(Type t)
            {
                if (t.IsEnum)
                {
                    var values = Enum.GetValues(t).Cast<int>();
                    var names = Enum.GetNames(t);
                    return " // " + string.Join(", ", names.Zip(values, (a, b) => $"{a} = {b:D}"));
                }

                return "";
            }
        }


        private static T CastByExample<T>([NotNull] T example, [NotNull] object obj)
        {
            // example above is just for compiler magic
            // to infer the type to cast obj to
            return (T) obj;
        }

        #region Converters

        private class VariableListConverter : JsonConverter<List<ProfileSchema.Variable>>
        {
            private List<AtomBaseVariable> _hints;

            public VariableListConverter(List<AtomBaseVariable> hints)
            {
                _hints = hints;
            }

            public override void WriteJson(
                JsonWriter writer,
                List<ProfileSchema.Variable> variables,
                JsonSerializer serializer
            )
            {
                writer.WriteStartArray();
                foreach (ProfileSchema.Variable variable in variables)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(nameof(ProfileSchema.Variable.Name));
                    serializer.Serialize(writer, variable.Name);
                    writer.WritePropertyName(nameof(ProfileSchema.Variable.Value));
                    serializer.Serialize(writer, variable.Value);
                    writer.WritePropertyName(nameof(ProfileSchema.Variable.Type));
                    serializer.Serialize(writer, variable.Type);
                    writer.WritePropertyName(nameof(ProfileSchema.Variable.Fingerprint));
                    serializer.Serialize(writer, variable.Fingerprint);
                    writer.WritePropertyName(nameof(ProfileSchema.Variable.Description));
                    serializer.Serialize(writer, variable.Description);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            public override List<ProfileSchema.Variable> ReadJson(
                JsonReader reader,
                Type valueType,
                List<ProfileSchema.Variable> variables,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                const string valueKey = nameof(ProfileSchema.Variable.Value);
                const string fingerprintKey = nameof(ProfileSchema.Variable.Fingerprint);
                JArray jArray = JArray.Load(reader);
                var result = new List<ProfileSchema.Variable>();
                foreach (JToken token in jArray)
                {
                    var jObject = token as JObject;
                    Debug.Assert(jObject != null, "Suppose we know the data structure");
                    var fingerprint = jObject[fingerprintKey]?.Value<string>();
                    AtomBaseVariable match = _hints.FirstOrDefault(v => v.Id == fingerprint);
                    if (match != null)
                    {
                        var variable = new ProfileSchema.Variable
                        {
                            Fingerprint = jObject[fingerprintKey]?.ToString(),
                            Value = match.IsPrimitive() || match.IsValueType()
                                ? ToValue(jObject, valueKey, match)
                                : jObject[valueKey],
                        };
                        result.Add(variable);
                    }
                    else
                    {
                        Debug.Log($"[{nameof(VariablePersistence)}] : Data for unknown channel {fingerprint} ignored.");
                    }
                }

                return result;
            }

            private static object ToValue(JObject jObject, string valueKey, AtomBaseVariable match)
            {
                return jObject[valueKey]?.ToObject(match.BaseValue.GetType());
            }
        }

        #endregion

        private void AssignToObject(JToken token, object target, Type targetType)
        {
            var serializer = JsonSerializer.Create(_serializerSettings);
            var source = token as JObject;
            TryAssignToProperties(source, target, targetType, serializer);
            TryAssignToFields(source, target, targetType, serializer);
        }

        private static void TryAssignToFields(
            JObject source,
            object target,
            IReflect targetType,
            JsonSerializer serializer
        )
        {
            if (source == null)
                return;

            // try assign fields
            var fieldInfos = targetType.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (source.ContainsKey(fieldInfo.Name))
                {
                    var jValue = source[fieldInfo.Name]
                        .ToObject(fieldInfo.FieldType, serializer);
                    fieldInfo.SetValue(target, jValue);
                    // Debug.Log($"Field {fieldInfo.Name} = {jValue}");
                }
                else
                {
                    // Debug.Log($"Field {fieldInfo.Name} ignored");
                }
            }
        }

        private static void TryAssignToProperties(
            JObject source,
            object target,
            Type targetType,
            JsonSerializer serializer
        )
        {
            bool IsNotUnityObjectProperty(PropertyInfo i)
                => !targetType.InheritsFrom(typeof(Object)) ||
                   !i.Name.Equals(nameof(Object.hideFlags)) && !i.Name.Equals(nameof(Object.name));

            var propInfos = targetType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public)
                .Where(IsNotUnityObjectProperty);

            foreach (PropertyInfo propInfo in propInfos)
            {
                if (source == null)
                {
                    Debug.Log($"Property {targetType?.Name}.{propInfo?.Name} ignored (Source is null)");
                    continue;
                }

                if (propInfo == null)
                {
                    Debug.Log($"Property {targetType?.Name} ignored (PropertyInfo is null");
                    continue;
                }

                if (!source.ContainsKey(propInfo.Name))
                {
                    Debug.Log(
                        $"Property {targetType?.Name}.{propInfo?.Name} ignored (Source doesn't contain property name). Source:\n{source}");
                    continue;
                }

                if (source[propInfo.Name] == null)
                {
                    Debug.Log(
                        $"Property {targetType?.Name}.{propInfo?.Name} ignored (Source doesn't contain property name). Source:\n{source}");
                    continue;
                }

                var jValue = source[propInfo.Name].ToObject(propInfo.PropertyType, serializer);
                propInfo.SetValue(target, jValue);
                Debug.Log($"{propInfo.DeclaringType} Property {propInfo.Name} = {jValue}");
            }
        }

        public static string ToAscii(string str)
        {
            var asAscii = Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()
                    ),
                    Encoding.UTF8.GetBytes(str)
                )
            );
            return str;
        }

        private void OnDisable()
        {
            if (saveNow != null)
                saveNow.Unregister(SaveCurrentProfile);
            if (loadNow != null)
                loadNow.Unregister(LoadAndApplyNow);

            if (saveOnDestroy)
                SaveCurrentProfile(keepVersion);
        }

        private void OnDestroy()
        {
        }

        private class ProfileSchema
        {
            public string Description =
                $"Generated by {Application.productName} {Application.version} on {DateTime.UtcNow:yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK}";

            public Context Meta;
            public List<ProfileSchema.Variable> Variables;

            public class Context
            {
                public string CompanyName;
                public string ProductName;
                public string Version;
                public string BuildGUID;
                public string Identifier;
                public string Platform;
                public bool Genuine;
                public string UnityVersion;
                public string SystemLanguage;
                public DateTime Utc;
                public float Runtime;
            }

            public class Variable
            {
                public string Name;
                public object Value;
                public string Type;
                public string Fingerprint;
                public string Description;
            }
        }

        public static string Redact(string str)
        {
            return
                str.Length > 12 ? $"{str.Substring(0, 4)}...{str.Substring(str.Length - 4, 4)}" :
                str.Length > 8 ? $"{str.Substring(0, 3)}...{str.Substring(str.Length - 3, 3)}" :
                str.Length > 6 ? $"{str.Substring(0, 2)}...{str.Substring(str.Length - 2, 2)}" :
                str.Length > 3 ? $"{str.Substring(0, 1)}...{str.Substring(str.Length - 1, 1)}" :
                "...";
        }
    }

    internal enum FileLocation
    {
        PersistentDataPath,
        DataPath,
        StreamingAssetsPath,
        AbsolutePath,
    }
}