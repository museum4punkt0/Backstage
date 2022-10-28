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
    /// Individual media assets
    /// </summary>
    [GeneratedRecord("assets")]
    public partial class @AssetsRecord : DbRecord, IDbRecord    { // start of generated class

        public static explicit operator AssetsRecord(long id) => DbRecordUtil.CreateRef<AssetsRecord>(id);
        public static explicit operator AssetsRecord(string id) => DbRecordUtil.CreateRef<AssetsRecord>(id);

        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "copyright", Required = Required.Default)]
        public System.String @Copyright { get; set; } // directus type: Text
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "scientific_name", Required = Required.Default)]
        public System.String @ScientificName { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "discovery_year", Required = Required.Default)]
        public System.Int32 @DiscoveryYear { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "discovery_location", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<LocationsRecord>))]  
        public LocationsRecord @DiscoveryLocation { get; set; } // directus type: Integer
        
        /// <summary>
        /// Die Asset-Datei muss dem unten ausgewählten Asset-Typ entsprechen. Falsch konfigurierte Assets werden ignoriert.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "asset", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<DirectusFile>))]  
        public DirectusFile @Asset { get; set; } // directus type: Uuid
        
        [Serializable]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeChoices
        {

            [EnumMember(Value = "model")]
            Model,
            [EnumMember(Value = "image")]
            Image,
            [EnumMember(Value = "video")]
            Video
        }
        /// <summary>
        /// Der Asset-Typ muss mit der oben ausgewählten Asset-Datei übereinstimmen. Falsch konfigurierte Assets werden ignoriert.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "type", Required = Required.Default)]
        public TypeChoices @Type { get; set; } // directus type: String
        
        [Serializable]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum LicenseChoices
        {

            [EnumMember(Value = "none")]
            None,
            [EnumMember(Value = "cc_1_0")]
            Cc10,
            [EnumMember(Value = "cc_by_3_0")]
            CcBy30,
            [EnumMember(Value = "cc_by_4_0")]
            CcBy40,
            [EnumMember(Value = "cc_by_4_1")]
            CcBy41,
            [EnumMember(Value = "cc_by_4_2")]
            CcBy42,
            [EnumMember(Value = "cc_by_4_3")]
            CcBy43
        }
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "license", Required = Required.Default)]
        public LicenseChoices @License { get; set; } // directus type: String
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "is_artefact", Required = Required.Default)]
        public System.Boolean @IsArtefact { get; set; } // directus type: Boolean
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<AssetsTranslationsRecord> @Translations { get; set; } // directus type: Translations
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "artefacts", Required = Required.Default)]
        [JsonIgnore] public AssetsArtefactsRecord[] __ArtefactsJunction { get; set; } // directus type: ManyToMany
        [JsonIgnore] public ArtefactsRecord[] @Artefacts => __ArtefactsCache ?? (__ArtefactsCache = __ArtefactsJunction?.Select(it => it.ArtefactsId).ToArray());
        [JsonIgnore] private ArtefactsRecord[] __ArtefactsCache;
        
       
        [JsonIgnore] public string __Table => "assets";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(AssetsRecord);
    } // end of generated class
}
