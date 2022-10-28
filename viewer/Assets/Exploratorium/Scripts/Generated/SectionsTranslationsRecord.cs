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
    [GeneratedRecord("sections_translations")]
    public partial class @SectionsTranslationsRecord : DbRecord, IDbRecord, ITranslationRecord    { // start of generated class

        public static explicit operator SectionsTranslationsRecord(long id) => DbRecordUtil.CreateRef<SectionsTranslationsRecord>(id);
        public static explicit operator SectionsTranslationsRecord(string id) => DbRecordUtil.CreateRef<SectionsTranslationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections_translations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 145,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections_translations",        ///   "field": "languages_code",        ///   "hidden": true,        ///   "id": 146,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "sections_translations",        ///   "field": "sections_id",        ///   "hidden": true,        ///   "id": 147,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "sections_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @SectionsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// Sektionen Beschreibung, Darstellung von max. 1250 Zeichen in max. 8 Zeilen (1 Zeile ca. 160 Zeichen).
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections_translations",        ///   "field": "text",        ///   "hidden": false,        ///   "id": 175,        ///   "interface": "input-rich-text-md",        ///   "note": "Sektionen Beschreibung, Darstellung von max. 1250 Zeichen in max. 8 Zeilen (1 Zeile ca. 160 Zeichen).",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 5,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "text", Required = Required.Default)]
        public Markdig.Syntax.MarkdownDocument @Text { get; set; } // directus type: Text
        
        /// <summary>
        /// Sektionen Name 
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections_translations",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 230,        ///   "interface": "input",        ///   "note": "Sektionen Name ",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 4,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public System.String @Name { get; set; } // directus type: String
        
        /// <summary>
        /// This is expected to be a video asset. It will only be played for sections that are immediate children of the "Topics" section
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "sections_translations",        ///   "field": "intro",        ///   "hidden": false,        ///   "id": 316,        ///   "interface": "select-dropdown-m2o",        ///   "note": "This is expected to be a video asset. It will only be played for sections that are immediate children of the \"Topics\" section",        ///   "options": {        ///     "template": "{{type}} {{name}}"        ///   },        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "intro", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<AssetsRecord>))]  
        public AssetsRecord @Intro { get; set; } // directus type: Integer
        [JsonIgnore] public string __Locale => LanguagesCode?.__Primary;
       
        [JsonIgnore] public string __Table => "sections_translations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(SectionsTranslationsRecord);
    } // end of generated class
}

