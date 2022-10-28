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
    [GeneratedRecord("artefacts_tags_1")]
    public partial class @ArtefactsTags1Record : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator ArtefactsTags1Record(long id) => DbRecordUtil.CreateRef<ArtefactsTags1Record>(id);
        public static explicit operator ArtefactsTags1Record(string id) => DbRecordUtil.CreateRef<ArtefactsTags1Record>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts_tags_1",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 216,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "artefacts_tags_1",        ///   "field": "tags_id",        ///   "hidden": true,        ///   "id": 218,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "tags_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<TagsRecord>))]  
        public TagsRecord @TagsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "artefacts_tags_1",        ///   "field": "artefacts_id",        ///   "hidden": true,        ///   "id": 219,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "artefacts_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<ArtefactsRecord>))]  
        public ArtefactsRecord @ArtefactsId { get; set; } // directus type: Integer
       
        [JsonIgnore] public string __Table => "artefacts_tags_1";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(ArtefactsTags1Record);
    } // end of generated class
}

