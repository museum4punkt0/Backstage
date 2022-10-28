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
    /// Language definitions
    /// </summary>
    [GeneratedRecord("languages")]
    public partial class @LanguagesRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator LanguagesRecord(long id) => DbRecordUtil.CreateRef<LanguagesRecord>(id);
        public static explicit operator LanguagesRecord(string id) => DbRecordUtil.CreateRef<LanguagesRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "languages",        ///   "field": "code",        ///   "hidden": false,        ///   "id": 128,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "iconLeft": "vpn_key"        ///   },        ///   "readonly": false,        ///   "sort": 1,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public System.String @Code { get; set; } // directus type: String
      

        [JsonIgnore] public override string __Primary 
        {
            get => @Code; 
            set => @Code = value;
        }
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "languages",        ///   "field": "name",        ///   "hidden": false,        ///   "id": 129,        ///   "interface": "input",        ///   "note": null,        ///   "options": {        ///     "iconLeft": "translate"        ///   },        ///   "readonly": false,        ///   "sort": 2,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "half"        /// }
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
        /// {        ///   "collection": "languages",        ///   "field": "hyphenation_patterns",        ///   "hidden": false,        ///   "id": 263,        ///   "interface": "file",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 3,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "hyphenation_patterns", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @HyphenationPatterns { get; set; } // directus type: Uuid
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "languages",        ///   "field": "hyphenation_exceptions",        ///   "hidden": false,        ///   "id": 265,        ///   "interface": "file",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 4,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "hyphenation_exceptions", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @HyphenationExceptions { get; set; } // directus type: Uuid
       
        [JsonIgnore] public string __Table => "languages";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(LanguagesRecord);
    } // end of generated class
}

