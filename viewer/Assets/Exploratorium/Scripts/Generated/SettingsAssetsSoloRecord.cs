/* Auto-Generated on 03/25/2022 19:17:29 +00:00 
   GeneratorAssembly = "Directus.Connect.v9";        
   GeneratorAssemblyVersion = "1.3.803.686";

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
    [GeneratedRecord("settings_assets_solo")]
    public partial class @SettingsAssetsSoloRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator SettingsAssetsSoloRecord(long id) => DbRecordUtil.CreateRef<SettingsAssetsSoloRecord>(id);
        public static explicit operator SettingsAssetsSoloRecord(string id) => DbRecordUtil.CreateRef<SettingsAssetsSoloRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings_assets_solo",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 290,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "settings_assets_solo",        ///   "field": "settings_id",        ///   "hidden": true,        ///   "id": 291,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "settings_assets_solo",        ///   "field": "assets_id",        ///   "hidden": true,        ///   "id": 292,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "assets_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<AssetsRecord>))]  
        public AssetsRecord @AssetsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings_assets_solo",        ///   "field": "sort",        ///   "hidden": true,        ///   "id": 293,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "sort", Required = Required.Default)]
        public System.Int32 @Sort { get; set; } // directus type: Integer
       
        [JsonIgnore] public string __Table => "settings_assets_solo";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(SettingsAssetsSoloRecord);
    } // end of generated class
}

