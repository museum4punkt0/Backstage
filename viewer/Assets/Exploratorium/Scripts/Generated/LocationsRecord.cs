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
    /// Geographic locations of assets
    /// </summary>
    [GeneratedRecord("locations")]
    public partial class @LocationsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator LocationsRecord(long id) => DbRecordUtil.CreateRef<LocationsRecord>(id);
        public static explicit operator LocationsRecord(string id) => DbRecordUtil.CreateRef<LocationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "locations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 198,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "status",        ///   "hidden": false,        ///   "id": 199,        ///   "interface": "select-dropdown",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Published",        ///         "value": "published"        ///       },        ///       {        ///         "text": "Draft",        ///         "value": "draft"        ///       },        ///       {        ///         "text": "Archived",        ///         "value": "archived"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "sort",        ///   "hidden": true,        ///   "id": 200,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "user_created",        ///   "hidden": true,        ///   "id": 201,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 4,        ///   "special": [        ///     "user-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "date_created",        ///   "hidden": true,        ///   "id": 202,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 5,        ///   "special": [        ///     "date-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "user_updated",        ///   "hidden": true,        ///   "id": 203,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 6,        ///   "special": [        ///     "user-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "date_updated",        ///   "hidden": true,        ///   "id": 204,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 7,        ///   "special": [        ///     "date-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 205,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "font": "monospace"        ///   },        ///   "readonly": false,        ///   "sort": 8,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "thumbnail",        ///   "hidden": false,        ///   "id": 206,        ///   "interface": "file-image",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 10,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "locations",        ///   "field": "latitude",        ///   "hidden": false,        ///   "id": 207,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "min": "-180",        ///     "max": "180",        ///     "step": "0.1",        ///     "iconLeft": "grid_on"        ///   },        ///   "readonly": false,        ///   "sort": 11,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "latitude", Required = Required.Default)]
        public System.Single @Latitude { get; set; } // directus type: Float
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "locations",        ///   "field": "longitude",        ///   "hidden": false,        ///   "id": 208,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "step": "0.1",        ///     "min": "-90",        ///     "max": "90",        ///     "iconLeft": "grid_on"        ///   },        ///   "readonly": false,        ///   "sort": 12,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "longitude", Required = Required.Default)]
        public System.Single @Longitude { get; set; } // directus type: Float
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "locations",        ///   "field": "assets",        ///   "hidden": false,        ///   "id": 248,        ///   "interface": "list-o2m",        ///   "note": null,        ///   "options": {        ///     "template": "{{thumbnail.$thumbnail}}  {{type}}  {{name}}",        ///     "enableCreate": false        ///   },        ///   "readonly": false,        ///   "sort": 14,        ///   "special": [        ///     "o2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "assets", Required = Required.Default)]
        [JsonIgnore]
        public AssetsRecord[] @Assets { get; set; } // directus type: OneToMany
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "locations",        ///   "field": "translations",        ///   "hidden": false,        ///   "id": 241,        ///   "interface": "translations",        ///   "note": null,        ///   "options": {        ///     "languageTemplate": "{{name}}",        ///     "translationsTemplate": "{{country}} /  {{place}}"        ///   },        ///   "readonly": false,        ///   "sort": 13,        ///   "special": [        ///     "translations"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<LocationsTranslationsRecord> @Translations { get; set; } // directus type: Translations
       
        [JsonIgnore] public string __Table => "locations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(LocationsRecord);
    } // end of generated class
}

