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
    [GeneratedRecord("persons_translations")]
    public partial class @PersonsTranslationsRecord : DbRecord, IDbRecord, ITranslationRecord    { // start of generated class

        public static explicit operator PersonsTranslationsRecord(long id) => DbRecordUtil.CreateRef<PersonsTranslationsRecord>(id);
        public static explicit operator PersonsTranslationsRecord(string id) => DbRecordUtil.CreateRef<PersonsTranslationsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "persons_translations",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 141,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "persons_translations",        ///   "field": "languages_code",        ///   "hidden": true,        ///   "id": 142,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "persons_translations",        ///   "field": "persons_id",        ///   "hidden": true,        ///   "id": 143,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "persons_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<PersonsRecord>))]  
        public PersonsRecord @PersonsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// Name inklusive Titel
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "persons_translations",        ///   "field": "title",        ///   "hidden": false,        ///   "id": 167,        ///   "interface": "input",        ///   "note": "Name inklusive Titel",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 4,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "title", Required = Required.Default)]
        public System.String @Title { get; set; } // directus type: String
        
        /// <summary>
        /// Darstellung von max. 1250 Zeichen in max. 8 Zeilen (1 Zeile ca. 160 Zeichen).
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "persons_translations",        ///   "field": "description",        ///   "hidden": true,        ///   "id": 271,        ///   "interface": "input-rich-text-md",        ///   "note": "Darstellung von max. 1250 Zeichen in max. 8 Zeilen (1 Zeile ca. 160 Zeichen).",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 6,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "description", Required = Required.Default)]
        public Markdig.Syntax.MarkdownDocument @Description { get; set; } // directus type: Text
        
        /// <summary>
        /// Stellenbezeichnung
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "persons_translations",        ///   "field": "role",        ///   "hidden": false,        ///   "id": 272,        ///   "interface": "input-rich-text-md",        ///   "note": "Stellenbezeichnung",        ///   "options": null,        ///   "readonly": false,        ///   "sort": 5,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "role", Required = Required.Default)]
        public Markdig.Syntax.MarkdownDocument @Role { get; set; } // directus type: Text
        [JsonIgnore] public string __Locale => LanguagesCode?.__Primary;
       
        [JsonIgnore] public string __Table => "persons_translations";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(PersonsTranslationsRecord);
    } // end of generated class
}

