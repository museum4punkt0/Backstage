/* Auto-Generated on 03/28/2022 19:07:32 +00:00 
   GeneratorAssembly = "Directus.Connect.v9";        
   GeneratorAssemblyVersion = "1.3.810.600";

*/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Directus.Connect.v9;
using Directus.Connect.v9.Converters;
using Directus.Connect.v9.CodeGeneration;

namespace Directus.Generated
{

    /// <summary>
    /// No comment.
    /// </summary>
    [GeneratedRecord("settings_translations")]
    public partial class @SettingsTranslationsRecord : DbRecord, IDbRecord, ITranslationRecord    { // start of generated class

        public static explicit operator SettingsTranslationsRecord(long id) => DbRecordUtil.CreateRef<SettingsTranslationsRecord>(id);
        public static explicit operator SettingsTranslationsRecord(string id) => DbRecordUtil.CreateRef<SettingsTranslationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings_translations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 302,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public System.Int32 @Id { get; set; } // directus type: Integer
      

        [JsonIgnore] public override string __Primary 
        {
            get => @Id.ToString(); 
            set => @Id = int.Parse(value);
        }
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings_translations",        ///   "field": "languages_code",        ///   "hidden": true,        ///   "id": 303,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "languages_code", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<LanguagesRecord>))]  
        public LanguagesRecord @LanguagesCode { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings_translations",        ///   "field": "settings_id",        ///   "hidden": true,        ///   "id": 304,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "settings_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SettingsRecord>))]  
        public SettingsRecord @SettingsId { get; set; } // directus type: Uuid
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings_translations",        ///   "field": "idle_assetpool_solo",        ///   "hidden": false,        ///   "id": 305,        ///   "interface": "list-m2m",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 4,        ///   "special": [        ///     "m2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "idle_assetpool_solo", Required = Required.Default)]
        [JsonIgnore] public SettingsTranslationsAssetsRecord[] __IdleAssetpoolSoloJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public AssetsRecord[] @IdleAssetpoolSolo => __IdleAssetpoolSoloCache ?? (__IdleAssetpoolSoloCache = __IdleAssetpoolSoloJunction?.Select(it => it.AssetsId).ToArray());
        [JsonIgnore] private AssetsRecord[] __IdleAssetpoolSoloCache;
        
        [JsonIgnore] public string __Locale => LanguagesCode?.__Primary;
       
        [JsonIgnore] public string __Table => "settings_translations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(SettingsTranslationsRecord);
    } // end of generated class
}

