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
    [GeneratedRecord("artefacts_translations")]
    public partial class @ArtefactsTranslationsRecord : DbRecord, IDbRecord, ITranslationRecord    { // start of generated class

        public static explicit operator ArtefactsTranslationsRecord(long id) => DbRecordUtil.CreateRef<ArtefactsTranslationsRecord>(id);
        public static explicit operator ArtefactsTranslationsRecord(string id) => DbRecordUtil.CreateRef<ArtefactsTranslationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts_translations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 127,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts_translations",        ///   "field": "artefacts_id",        ///   "hidden": true,        ///   "id": 130,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "artefacts_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<ArtefactsRecord>))]  
        public ArtefactsRecord @ArtefactsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts_translations",        ///   "field": "languages_id",        ///   "hidden": true,        ///   "id": 131,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "languages_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<LanguagesRecord>))]  
        public LanguagesRecord @LanguagesId { get; set; } // directus type: String
        
        /// <summary>
        /// Objekt Beschreibung (Infotext), Darstellung von max. 1250 Zeichen in max. 8 Zeilen (1 Zeile ca. 160 Zeichen).
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts_translations",        ///   "field": "text",        ///   "hidden": false,        ///   "id": 152,        ///   "interface": "input-rich-text-md",        ///   "note": "Objekt Beschreibung (Infotext), Darstellung von max. 1250 Zeichen in max. 8 Zeilen (1 Zeile ca. 160 Zeichen).",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 6,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts_translations",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 178,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 4,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public System.String @Name { get; set; } // directus type: String
        
        /// <summary>
        /// Objekt Titel (Headline), Darstellung von max. 80 Zeichen in max. 3 Zeilen, empfohlen max. 35 Zeichen pro Zeile.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts_translations",        ///   "field": "title",        ///   "hidden": false,        ///   "id": 262,        ///   "interface": "input-multiline",        ///   "note": "Objekt Titel (Headline), Darstellung von max. 80 Zeichen in max. 3 Zeilen, empfohlen max. 35 Zeichen pro Zeile.",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 5,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "title", Required = Required.Default)]
        public System.String @Title { get; set; } // directus type: Text
        [JsonIgnore] public string __Locale => LanguagesId?.__Primary;
       
        [JsonIgnore] public string __Table => "artefacts_translations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(ArtefactsTranslationsRecord);
    } // end of generated class
}

