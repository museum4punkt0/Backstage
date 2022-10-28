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
    /// Categories for objects
    /// </summary>
    [GeneratedRecord("sections")]
    public partial class @SectionsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator SectionsRecord(long id) => DbRecordUtil.CreateRef<SectionsRecord>(id);
        public static explicit operator SectionsRecord(string id) => DbRecordUtil.CreateRef<SectionsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 43,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "status",        ///   "hidden": false,        ///   "id": 44,        ///   "interface": "select-dropdown",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Published",        ///         "value": "published"        ///       },        ///       {        ///         "text": "Draft",        ///         "value": "draft"        ///       },        ///       {        ///         "text": "Archived",        ///         "value": "archived"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "sort",        ///   "hidden": true,        ///   "id": 45,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "user_created",        ///   "hidden": true,        ///   "id": 46,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 4,        ///   "special": [        ///     "user-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "date_created",        ///   "hidden": true,        ///   "id": 47,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 5,        ///   "special": [        ///     "date-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "user_updated",        ///   "hidden": true,        ///   "id": 48,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 6,        ///   "special": [        ///     "user-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "date_updated",        ///   "hidden": true,        ///   "id": 49,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 7,        ///   "special": [        ///     "date-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 50,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "iconLeft": "vpn_key"        ///   },        ///   "readonly": false,        ///   "sort": 8,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "thumbnail",        ///   "hidden": false,        ///   "id": 173,        ///   "interface": "file-image",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 9,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections",        ///   "field": "color",        ///   "hidden": false,        ///   "id": 176,        ///   "interface": "select-color",        ///   "note": null,        ///   "options": {        ///     "presets": [        ///       {        ///         "name": "yellow",        ///         "color": "#EBCF2D"        ///       },        ///       {        ///         "name": "red",        ///         "color": "#E5633D"        ///       },        ///       {        ///         "name": "green",        ///         "color": "#72B667"        ///       },        ///       {        ///         "color": "#5BBFC1",        ///         "name": "teal"        ///       },        ///       {        ///         "name": "blue",        ///         "color": "#5BA4D9"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 13,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "color", Required = Required.Default)]
        public System.String @Color { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections",        ///   "field": "parent",        ///   "hidden": false,        ///   "id": 228,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 15,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "parent", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @Parent { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections",        ///   "field": "translations",        ///   "hidden": false,        ///   "id": 144,        ///   "interface": "translations",        ///   "note": null,        ///   "options": {        ///     "languageTemplate": "{{name}}",        ///     "translationsTemplate": "{{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 10,        ///   "special": [        ///     "translations"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<SectionsTranslationsRecord> @Translations { get; set; } // directus type: Translations
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections",        ///   "field": "artefacts",        ///   "hidden": true,        ///   "id": 224,        ///   "interface": "list-o2m",        ///   "note": null,        ///   "options": {        ///     "template": "{{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 14,        ///   "special": [        ///     "o2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "artefacts", Required = Required.Default)]
        [JsonIgnore]
        public ArtefactsRecord[] @Artefacts { get; set; } // directus type: OneToMany
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections",        ///   "field": "children",        ///   "hidden": false,        ///   "id": 229,        ///   "interface": "list-o2m-tree-view",        ///   "note": null,        ///   "options": {        ///     "displayTemplate": "{{parent.name}} / {{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 16,        ///   "special": [        ///     "o2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "children", Required = Required.Default)]
        [JsonIgnore]
        public SectionsRecord[] @Children { get; set; } // directus type: OneToMany
       
        [JsonIgnore] public string __Table => "sections";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(SectionsRecord);
    } // end of generated class
}

