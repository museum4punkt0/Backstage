/* Auto-Generated on 10/28/2021 17:48:41 +00:00 
   GeneratorAssembly = "Directus.Connect.v9";        
   GeneratorAssemblyVersion = "1.3.652.710";

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
    [GeneratedRecord("images_translations")]
    public partial class @ImagesTranslationsRecord : DbRecord, IDbRecord, ITranslationRecord    { // start of generated class

        public static explicit operator ImagesTranslationsRecord(long id) => DbRecordUtil.CreateRef<ImagesTranslationsRecord>(id);
        public static explicit operator ImagesTranslationsRecord(string id) => DbRecordUtil.CreateRef<ImagesTranslationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "images_translations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 133,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "images_translations",        ///   "field": "languages_code",        ///   "hidden": true,        ///   "id": 134,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "images_translations",        ///   "field": "images_id",        ///   "hidden": true,        ///   "id": 135,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "images_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<ImagesRecord>))]  
        public ImagesRecord @ImagesId { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "images_translations",        ///   "field": "text",        ///   "hidden": false,        ///   "id": 153,        ///   "interface": "input-rich-text-md",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "images_translations",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 179,        ///   "interface": "input",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public System.String @Name { get; set; } // directus type: String
        [JsonIgnore] public string __Locale => LanguagesCode?.__Primary;
       
        [JsonIgnore] public string __Table => "images_translations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(ImagesTranslationsRecord);
    } // end of generated class
}

