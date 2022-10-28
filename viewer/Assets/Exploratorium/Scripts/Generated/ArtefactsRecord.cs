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
    /// Content items (aggregations of assets)
    /// </summary>
    [GeneratedRecord("artefacts")]
    public partial class @ArtefactsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator ArtefactsRecord(long id) => DbRecordUtil.CreateRef<ArtefactsRecord>(id);
        public static explicit operator ArtefactsRecord(string id) => DbRecordUtil.CreateRef<ArtefactsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 23,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "status",        ///   "hidden": false,        ///   "id": 24,        ///   "interface": "select-dropdown",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Published",        ///         "value": "published"        ///       },        ///       {        ///         "text": "Draft",        ///         "value": "draft"        ///       },        ///       {        ///         "text": "Archived",        ///         "value": "archived"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 7,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "sort",        ///   "hidden": true,        ///   "id": 25,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "user_created",        ///   "hidden": true,        ///   "id": 26,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 3,        ///   "special": [        ///     "user-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "date_created",        ///   "hidden": true,        ///   "id": 27,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 5,        ///   "special": [        ///     "date-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "user_updated",        ///   "hidden": true,        ///   "id": 28,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 4,        ///   "special": [        ///     "user-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "date_updated",        ///   "hidden": true,        ///   "id": 29,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 6,        ///   "special": [        ///     "date-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "date_updated", Required = Required.Default)]
        public System.DateTimeOffset @DateUpdated { get; set; } // directus type: Timestamp
        
        /// <summary>
        /// Ein kurzer beschreibender Name zur Identifizierung eines Datensatzes.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 30,        ///   "interface": "input",        ///   "note": "Ein kurzer beschreibender Name zur Identifizierung eines Datensatzes.",        ///   "options": {        ///     "font": "monospace",        ///     "iconLeft": "vpn_key"        ///   },        ///   "readonly": false,        ///   "sort": 8,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "thumbnail",        ///   "hidden": false,        ///   "id": 157,        ///   "interface": "file-image",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 12,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "artefacts",        ///   "field": "section",        ///   "hidden": false,        ///   "id": 223,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{parent.name}} / {{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "section", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @Section { get; set; } // directus type: Integer
        
        [Serializable]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum LayoutChoices
        {

            [EnumMember(Value = "video")]
            Video,
            [EnumMember(Value = "slideshow")]
            Slideshow,
            [EnumMember(Value = "model")]
            Model
        }
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "layout",        ///   "hidden": false,        ///   "id": 232,        ///   "interface": "select-radio",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Video",        ///         "value": "video"        ///       },        ///       {        ///         "text": "Slideshow",        ///         "value": "slideshow"        ///       },        ///       {        ///         "text": "Model",        ///         "value": "model"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 13,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "layout", Required = Required.Default)]
        public LayoutChoices @Layout { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "topic",        ///   "hidden": false,        ///   "id": 252,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "topic", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @Topic { get; set; } // directus type: Integer
        
        /// <summary>
        /// Dieser Wert beeinflusst die Priorität, die dieser Artikel hat, wenn er als Teil einer Sammlung von Artikeln angezeigt wird. Ein höherer Wert bedeutet, dass dieser Artikel prominenter platziert wird.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "priority",        ///   "hidden": false,        ///   "id": 273,        ///   "interface": "slider",        ///   "note": "Dieser Wert beeinflusst die Priorität, die dieser Artikel hat, wenn er als Teil einer Sammlung von Artikeln angezeigt wird. Ein höherer Wert bedeutet, dass dieser Artikel prominenter platziert wird.",        ///   "options": {        ///     "minValue": 0,        ///     "maxValue": "1",        ///     "stepInterval": "1",        ///     "alwaysShowValue": true        ///   },        ///   "readonly": false,        ///   "sort": 9,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "priority", Required = Required.Default)]
        public System.Int32 @Priority { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "persons",        ///   "hidden": false,        ///   "id": 74,        ///   "interface": "list-m2m",        ///   "note": null,        ///   "options": {        ///     "template": "{{persons_id.thumbnail.$thumbnail}}  {{persons_id.name}}",        ///     "enableCreate": false        ///   },        ///   "readonly": false,        ///   "sort": 16,        ///   "special": [        ///     "m2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "persons", Required = Required.Default)]
        [JsonIgnore] public PersonsArtefactsRecord[] __PersonsJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public PersonsRecord[] @Persons => __PersonsCache ?? (__PersonsCache = __PersonsJunction?.Select(it => it.PersonsId).ToArray());
        [JsonIgnore] private PersonsRecord[] __PersonsCache;
        
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "assets",        ///   "hidden": false,        ///   "id": 112,        ///   "interface": "list-m2m",        ///   "note": null,        ///   "options": {        ///     "template": "{{assets_id.thumbnail.$thumbnail}}  {{assets_id.type}}  {{assets_id.name}}"        ///   },        ///   "readonly": false,        ///   "sort": 15,        ///   "special": [        ///     "m2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "assets", Required = Required.Default)]
        [JsonIgnore] public AssetsArtefactsRecord[] __AssetsJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public AssetsRecord[] @Assets => __AssetsCache ?? (__AssetsCache = __AssetsJunction?.Select(it => it.AssetsId).ToArray());
        [JsonIgnore] private AssetsRecord[] __AssetsCache;
        
        
        /// <summary>
        /// Objekttitel und -beschreibungen in Sprachen
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "translations",        ///   "hidden": false,        ///   "id": 126,        ///   "interface": "translations",        ///   "note": "Objekttitel und -beschreibungen in Sprachen",        ///   "options": {        ///     "translationsTemplate": "{{title}} / {{text}}",        ///     "languageTemplate": "{{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 14,        ///   "special": [        ///     "translations"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<ArtefactsTranslationsRecord> @Translations { get; set; } // directus type: Translations
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts",        ///   "field": "tags",        ///   "hidden": false,        ///   "id": 215,        ///   "interface": "list-m2m",        ///   "note": null,        ///   "options": {        ///     "template": "{{tags_id.name}}",        ///     "enableCreate": false        ///   },        ///   "readonly": false,        ///   "sort": 17,        ///   "special": [        ///     "m2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "tags", Required = Required.Default)]
        [JsonIgnore] public ArtefactsTags1Record[] __TagsJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public TagsRecord[] @Tags => __TagsCache ?? (__TagsCache = __TagsJunction?.Select(it => it.TagsId).ToArray());
        [JsonIgnore] private TagsRecord[] __TagsCache;
        
       
        [JsonIgnore] public string __Table => "artefacts";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(ArtefactsRecord);
    } // end of generated class
}

