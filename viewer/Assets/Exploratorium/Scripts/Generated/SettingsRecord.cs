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
    /// Global application settings
    /// </summary>
    [GeneratedRecord("settings")]
    public partial class @SettingsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator SettingsRecord(long id) => DbRecordUtil.CreateRef<SettingsRecord>(id);
        public static explicit operator SettingsRecord(string id) => DbRecordUtil.CreateRef<SettingsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 1,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 1,        ///   "special": [        ///     "uuid"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public System.String @Id { get; set; } // directus type: Uuid
      

        [JsonIgnore] public override string __Primary 
        {
            get => @Id; 
            set => @Id = value;
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
        /// {        ///   "collection": "settings",        ///   "field": "status",        ///   "hidden": true,        ///   "id": 2,        ///   "interface": "select-dropdown",        ///   "note": null,        ///   "options": {        ///     "choices": [        ///       {        ///         "text": "Published",        ///         "value": "published"        ///       },        ///       {        ///         "text": "Draft",        ///         "value": "draft"        ///       },        ///       {        ///         "text": "Archived",        ///         "value": "archived"        ///       }        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "settings",        ///   "field": "sort",        ///   "hidden": true,        ///   "id": 3,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "settings",        ///   "field": "user_created",        ///   "hidden": true,        ///   "id": 4,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 4,        ///   "special": [        ///     "user-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "settings",        ///   "field": "date_created",        ///   "hidden": true,        ///   "id": 5,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 5,        ///   "special": [        ///     "date-created"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "settings",        ///   "field": "user_updated",        ///   "hidden": true,        ///   "id": 6,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{avatar.$thumbnail}} {{first_name}} {{last_name}}"        ///   },        ///   "readonly": true,        ///   "sort": 6,        ///   "special": [        ///     "user-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "settings",        ///   "field": "date_updated",        ///   "hidden": true,        ///   "id": 7,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": true,        ///   "sort": 7,        ///   "special": [        ///     "date-updated"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "settings",        ///   "field": "section_root",        ///   "hidden": false,        ///   "id": 267,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{id}}  {{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 8,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "section_root", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @SectionRoot { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "topic_root",        ///   "hidden": false,        ///   "id": 268,        ///   "interface": "select-dropdown-m2o",        ///   "note": null,        ///   "options": {        ///     "template": "{{id}}  {{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 9,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "topic_root", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @TopicRoot { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "reset_timer",        ///   "hidden": false,        ///   "id": 269,        ///   "interface": "slider",        ///   "note": null,        ///   "options": {        ///     "minValue": "5",        ///     "maxValue": "600",        ///     "alwaysShowValue": true,        ///     "stepInterval": "1"        ///   },        ///   "readonly": false,        ///   "sort": 10,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "reset_timer", Required = Required.Default)]
        public System.Single @ResetTimer { get; set; } // directus type: Float
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "idle_screen",        ///   "hidden": false,        ///   "id": 270,        ///   "interface": "file-image",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "idle_screen", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @IdleScreen { get; set; } // directus type: Uuid
        
        /// <summary>
        /// Minimum Intervall zwischen den Videos im SOLO-Modus.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "idle_interval_min",        ///   "hidden": false,        ///   "id": 280,        ///   "interface": "slider",        ///   "note": "Minimum Intervall zwischen den Videos im SOLO-Modus.",        ///   "options": {        ///     "minValue": "10",        ///     "maxValue": "600",        ///     "stepInterval": "5"        ///   },        ///   "readonly": false,        ///   "sort": 5,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "idle_interval_min", Required = Required.Default)]
        public System.Int32 @IdleIntervalMin { get; set; } // directus type: Integer
        
        /// <summary>
        /// Maximal randomized Intervall, das zum minimalen Intervall zwischen den Videos im SOLO-Modus addiert wird.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "idle_interval_random",        ///   "hidden": false,        ///   "id": 281,        ///   "interface": "slider",        ///   "note": "Maximal randomized Intervall, das zum minimalen Intervall zwischen den Videos im SOLO-Modus addiert wird.",        ///   "options": {        ///     "minValue": 0,        ///     "maxValue": "600",        ///     "stepInterval": "5",        ///     "alwaysShowValue": true        ///   },        ///   "readonly": false,        ///   "sort": 6,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "idle_interval_random", Required = Required.Default)]
        public System.Int32 @IdleIntervalRandom { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "slideshow_autoplay",        ///   "hidden": false,        ///   "id": 294,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": [        ///     "boolean"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "slideshow_autoplay", Required = Required.Default)]
        public System.Boolean @SlideshowAutoplay { get; set; } // directus type: Boolean
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "video_autoplay",        ///   "hidden": false,        ///   "id": 295,        ///   "interface": "boolean",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 4,        ///   "special": [        ///     "boolean"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "video_autoplay", Required = Required.Default)]
        public System.Boolean @VideoAutoplay { get; set; } // directus type: Boolean
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "slideshow_default_showinfo",        ///   "hidden": false,        ///   "id": 297,        ///   "interface": "boolean",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": [        ///     "boolean"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "slideshow_default_showinfo", Required = Required.Default)]
        public System.Boolean @SlideshowDefaultShowinfo { get; set; } // directus type: Boolean
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "slideshow_slide_duration",        ///   "hidden": false,        ///   "id": 299,        ///   "interface": "slider",        ///   "note": null,        ///   "options": {        ///     "minValue": "5",        ///     "maxValue": "20",        ///     "stepInterval": "1",        ///     "alwaysShowValue": true        ///   },        ///   "readonly": false,        ///   "sort": 5,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "slideshow_slide_duration", Required = Required.Default)]
        public System.Single @SlideshowSlideDuration { get; set; } // directus type: Float
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "model_default_showinfo",        ///   "hidden": false,        ///   "id": 300,        ///   "interface": "boolean",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": [        ///     "boolean"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "model_default_showinfo", Required = Required.Default)]
        public System.Boolean @ModelDefaultShowinfo { get; set; } // directus type: Boolean
        
        /// <summary>
        /// Assets, die auf Station im OBSERVER-Modus angezeigt werden. Nur bildartige Assets sind erlaubt!
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "idle_assetpool_observer",        ///   "hidden": false,        ///   "id": 284,        ///   "interface": "list-m2m",        ///   "note": "Assets, die auf Station im OBSERVER-Modus angezeigt werden. Nur bildartige Assets sind erlaubt!",        ///   "options": {        ///     "enableCreate": false,        ///     "template": null        ///   },        ///   "readonly": false,        ///   "sort": 3,        ///   "special": [        ///     "m2m"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "idle_assetpool_observer", Required = Required.Default)]
        [JsonIgnore] public SettingsAssetsObserverRecord[] __IdleAssetpoolObserverJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public AssetsRecord[] @IdleAssetpoolObserver => __IdleAssetpoolObserverCache ?? (__IdleAssetpoolObserverCache = __IdleAssetpoolObserverJunction?.Select(it => it.AssetsId).ToArray());
        [JsonIgnore] private AssetsRecord[] __IdleAssetpoolObserverCache;
        
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "settings",        ///   "field": "translations",        ///   "hidden": false,        ///   "id": 301,        ///   "interface": "translations",        ///   "note": null,        ///   "options": {        ///     "languageTemplate": "{{name}}",        ///     "translationsTemplate": "{{idle_assetpool_solo}}"        ///   },        ///   "readonly": false,        ///   "sort": 14,        ///   "special": [        ///     "translations"        ///   ],        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<SettingsTranslationsRecord> @Translations { get; set; } // directus type: Translations
       
        [JsonIgnore] public string __Table => "settings";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(SettingsRecord);
    } // end of generated class
}

