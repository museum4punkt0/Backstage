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
    /// Individual media assets
    /// </summary>
    [GeneratedRecord("assets")]
    public partial class @AssetsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator AssetsRecord(long id) => DbRecordUtil.CreateRef<AssetsRecord>(id);
        public static explicit operator AssetsRecord(string id) => DbRecordUtil.CreateRef<AssetsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 85,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public System.Int32 @Id { get; set; } // directus type: Integer
      

        [JsonIgnore] public override string __Primary 
        {
            get => @Id.ToString(); 
            set => @Id = int.Parse(value);
        }
        
        [Serializable]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusChoices
        {

            [EnumMember(Value = "published")]
            Published,
            [EnumMember(Value = "draft")]
            Draft,
            [EnumMember(Value = "archived")]
            Archived
        }
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "status",        ///   "hidden": false,        ///   "id": 86,        ///   "interface": "select-dropdown",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Published",        ///         "value": "published"        ///       },        ///       {        ///         "text": "Draft",        ///         "value": "draft"        ///       },        ///       {        ///         "text": "Archived",        ///         "value": "archived"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public StatusChoices @Status { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "sort",        ///   "hidden": true,        ///   "id": 87,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "sort", Required = Required.Default)]
        public System.Int32 @Sort { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "user_created",        ///   "hidden": true,        ///   "id": 88,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 4,        ///   "special": [        ///     "user-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "user_created", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusUser>))]  
        public DirectusUser @UserCreated { get; set; } // directus type: Uuid
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "date_created",        ///   "hidden": true,        ///   "id": 89,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 5,        ///   "special": [        ///     "date-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "date_created", Required = Required.Default)]
        public System.DateTimeOffset @DateCreated { get; set; } // directus type: Timestamp
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "user_updated",        ///   "hidden": true,        ///   "id": 90,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 6,        ///   "special": [        ///     "user-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "user_updated", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusUser>))]  
        public DirectusUser @UserUpdated { get; set; } // directus type: Uuid
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "date_updated",        ///   "hidden": true,        ///   "id": 91,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 7,        ///   "special": [        ///     "date-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "date_updated", Required = Required.Default)]
        public System.DateTimeOffset @DateUpdated { get; set; } // directus type: Timestamp
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 156,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "iconLeft": "vpn_key",        ///     "font": "monospace"        ///   },        ///   "readonly": false,        ///   "sort": 8,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public System.String @Name { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "thumbnail",        ///   "hidden": false,        ///   "id": 163,        ///   "interface": "file-image",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 9,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "thumbnail", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @Thumbnail { get; set; } // directus type: Uuid
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "copyright",        ///   "hidden": false,        ///   "id": 231,        ///   "interface": "input-multiline",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "copyright", Required = Required.Default)]
        public System.String @Copyright { get; set; } // directus type: Text
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "scientific_name",        ///   "hidden": false,        ///   "id": 235,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "scientific_name", Required = Required.Default)]
        public System.String @ScientificName { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "discovery_year",        ///   "hidden": false,        ///   "id": 237,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "max": "9999",        ///     "min": "-9999",        ///     "iconLeft": "date_range"        ///   },        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "discovery_year", Required = Required.Default)]
        public System.Int32 @DiscoveryYear { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "discovery_location",        ///   "hidden": false,        ///   "id": 247,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 4,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "discovery_location", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<LocationsRecord>))]  
        public LocationsRecord @DiscoveryLocation { get; set; } // directus type: Integer
        
        /// <summary>
        /// Die Asset-Datei muss dem unten ausgewählten Asset-Typ entsprechen. Falsch konfigurierte Assets werden ignoriert.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "asset",        ///   "hidden": false,        ///   "id": 249,        ///   "interface": "file",        ///   "note": "Die Asset-Datei muss dem unten ausgewählten Asset-Typ entsprechen. Falsch konfigurierte Assets werden ignoriert.",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 10,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "asset", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @Asset { get; set; } // directus type: Uuid
        
        [Serializable]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeChoices
        {

            [EnumMember(Value = "model")]
            Model,
            [EnumMember(Value = "image")]
            Image,
            [EnumMember(Value = "video")]
            Video
        }
        /// <summary>
        /// Der Asset-Typ muss mit der oben ausgewählten Asset-Datei übereinstimmen. Falsch konfigurierte Assets werden ignoriert.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "type",        ///   "hidden": false,        ///   "id": 251,        ///   "interface": "select-radio",        ///   "note": "Der Asset-Typ muss mit der oben ausgewählten Asset-Datei übereinstimmen. Falsch konfigurierte Assets werden ignoriert.",        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Model",        ///         "value": "model"        ///       },        ///       {        ///         "text": "Image",        ///         "value": "image"        ///       },        ///       {        ///         "text": "Video",        ///         "value": "video"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 11,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "type", Required = Required.Default)]
        public TypeChoices @Type { get; set; } // directus type: String
        
        [Serializable]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum LicenseChoices
        {

            [EnumMember(Value = "none")]
            None,
            [EnumMember(Value = "cc_1_0")]
            Cc10,
            [EnumMember(Value = "cc_by_3_0")]
            CcBy30,
            [EnumMember(Value = "cc_by_4_0")]
            CcBy40,
            [EnumMember(Value = "cc_by_4_1")]
            CcBy41,
            [EnumMember(Value = "cc_by_4_2")]
            CcBy42,
            [EnumMember(Value = "cc_by_4_3")]
            CcBy43
        }
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "license",        ///   "hidden": false,        ///   "id": 253,        ///   "interface": "select-radio",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "value": "none",        ///         "text": "None"        ///       },        ///       {        ///         "text": "CC 1.0",        ///         "value": "cc_1_0"        ///       },        ///       {        ///         "text": "CC BY 3.0",        ///         "value": "cc_by_3_0"        ///       },        ///       {        ///         "text": "CC BY 4.0",        ///         "value": "cc_by_4_0"        ///       },        ///       {        ///         "text": "CC BY 4.1",        ///         "value": "cc_by_4_1"        ///       },        ///       {        ///         "text": "CC BY 4.2",        ///         "value": "cc_by_4_2"        ///       },        ///       {        ///         "value": "cc_by_4_3",        ///         "text": "CC BY 4.3"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "license", Required = Required.Default)]
        public LicenseChoices @License { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "is_artefact",        ///   "hidden": false,        ///   "id": 259,        ///   "interface": "boolean",        ///   "note": null,        ///   "options": {        ///     "label": "Artefact"        ///   },        ///   "readonly": false,        ///   "sort": 1,        ///   "special": [        ///     "boolean"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "is_artefact", Required = Required.Default)]
        public System.Boolean @IsArtefact { get; set; } // directus type: Boolean
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "translations",        ///   "hidden": false,        ///   "id": 132,        ///   "interface": "translations",        ///   "note": null,        ///   "options": {        ///     "languageTemplate": "{{name}}",        ///     "translationsTemplate": "{{title}} / {{text}}"        ///   },        ///   "readonly": false,        ///   "sort": 12,        ///   "special": [        ///     "translations"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<AssetsTranslationsRecord> @Translations { get; set; } // directus type: Translations
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets",        ///   "field": "artefacts",        ///   "hidden": false,        ///   "id": 109,        ///   "interface": "list-m2m",        ///   "note": null,        ///   "options": {        ///     "template": "{{artefacts_id.thumbnail.$thumbnail}} {{artefacts_id.layout}} {{artefacts_id.name}} "        ///   },        ///   "readonly": false,        ///   "sort": 15,        ///   "special": [        ///     "m2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "artefacts", Required = Required.Default)]
        [JsonIgnore] public AssetsArtefactsRecord[] __ArtefactsJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public ArtefactsRecord[] @Artefacts => __ArtefactsCache ?? (__ArtefactsCache = __ArtefactsJunction?.Select(it => it.ArtefactsId).ToArray());
        [JsonIgnore] private ArtefactsRecord[] __ArtefactsCache;
        
       
        [JsonIgnore] public string __Table => "assets";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(AssetsRecord);
    } // end of generated class
}

