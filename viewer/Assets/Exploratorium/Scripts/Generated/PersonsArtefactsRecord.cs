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
    [GeneratedRecord("persons_artefacts")]
    public partial class @PersonsArtefactsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator PersonsArtefactsRecord(long id) => DbRecordUtil.CreateRef<PersonsArtefactsRecord>(id);
        public static explicit operator PersonsArtefactsRecord(string id) => DbRecordUtil.CreateRef<PersonsArtefactsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "persons_artefacts",        ///   "field": "id",        ///   "hidden": true,        ///   "id": 72,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
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
        /// {        ///   "collection": "persons_artefacts",        ///   "field": "persons_id",        ///   "hidden": true,        ///   "id": 73,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "persons_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<PersonsRecord>))]  
        public PersonsRecord @PersonsId { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {        ///   "collection": "persons_artefacts",        ///   "field": "artefacts_id",        ///   "hidden": true,        ///   "id": 75,        ///   "interface": "datetime",        ///   "note": null,        ///   "options": null,        ///   "readonly": false,        ///   "sort": 0,        ///   "special": null,        ///   "system": false,        ///   "translation": null,        ///   "width": "full"        /// }
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "artefacts_id", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<ArtefactsRecord>))]  
        public ArtefactsRecord @ArtefactsId { get; set; } // directus type: Integer
       
        [JsonIgnore] public string __Table => "persons_artefacts";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(PersonsArtefactsRecord);
    } // end of generated class
}

