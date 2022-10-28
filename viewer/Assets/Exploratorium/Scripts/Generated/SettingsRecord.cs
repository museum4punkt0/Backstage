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
        /// {
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

        [JsonProperty(PropertyName = "section_root", Required = Required.Default)]
        [JsonConverter(typeof(DbRecordReferenceConverter<SectionsRecord>))]  
        public SectionsRecord @SectionRoot { get; set; } // directus type: Integer
        
        /// <summary>
        /// No comment.
        /// </summary>
        /// <remarks>
        /// <para>Field declaration:
        /// <code>
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
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
        /// {
        /// </code></para>
        /// </remarks>

        [JsonProperty(PropertyName = "translations", Required = Required.Default)]
        public Translations<SettingsTranslationsRecord> @Translations { get; set; } // directus type: Translations
       
        [JsonIgnore] public string __Table => "settings";
        [JsonIgnore] public static Type __TypeOfSelf => typeof(SettingsRecord);
    } // end of generated class
}
