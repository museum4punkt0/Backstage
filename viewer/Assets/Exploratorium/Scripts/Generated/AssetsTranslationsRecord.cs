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
    [GeneratedRecord("assets_translations")]
    public partial class @AssetsTranslationsRecord : DbRecord, IDbRecord, ITranslationRecord    { // start of generated class

        public static explicit operator AssetsTranslationsRecord(long id) => DbRecordUtil.CreateRef<AssetsTranslationsRecord>(id);
        public static explicit operator AssetsTranslationsRecord(string id) => DbRecordUtil.CreateRef<AssetsTranslationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets_translations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 133,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "assets_translations",        ///   "field": "languages_code",        ///   "hidden": true,        ///   "id": 134,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "assets_translations",        ///   "field": "assets_id",        ///   "hidden": true,        ///   "id": 135,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "assets_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<AssetsRecord>))]  
        public AssetsRecord @AssetsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// Asset Beschreibung (Infotext), bei Asset Video wird der Text nicht angezeigt. Für alle Informationen des Assets. Darstellung von max 800 Zeichen in max. 6 Zeilen.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets_translations",        ///   "field": "text",        ///   "hidden": false,        ///   "id": 153,        ///   "interface": "input-rich-text-md",        ///   "note": "Asset Beschreibung (Infotext), bei Asset Video wird der Text nicht angezeigt. Für alle Informationen des Assets. Darstellung von max 800 Zeichen in max. 6 Zeilen.",        ///   "options": {        ///     "toolbar": [        ///       "italic",        ///       "removeformat",        ///       "code"        ///     ]        ///   },        ///   "readonly": false,        ///   "sort": 6,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "text", Required = Required.Default)]
        public Markdig.Syntax.MarkdownDocument @Text { get; set; } // directus type: Text
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets_translations",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 179,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "font": "monospace"        ///   },        ///   "readonly": false,        ///   "sort": 4,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "assets_translations",        ///   "field": "subtitles",        ///   "hidden": false,        ///   "id": 250,        ///   "interface": "file",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "subtitles", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @Subtitles { get; set; } // directus type: Uuid
        
        /// <summary>
        /// Asset Title (nur intern, in Anwendung nicht sichtbar)
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "assets_translations",        ///   "field": "title",        ///   "hidden": false,        ///   "id": 254,        ///   "interface": "input-multiline",        ///   "note": "Asset Title (nur intern, in Anwendung nicht sichtbar)",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 5,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "title", Required = Required.Default)]
        public System.String @Title { get; set; } // directus type: Text
        [JsonIgnore] public string __Locale => LanguagesCode?.__Primary;
       
        [JsonIgnore] public string __Table => "assets_translations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(AssetsTranslationsRecord);
    } // end of generated class
}

