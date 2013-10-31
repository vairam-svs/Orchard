using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Orchard.Search {
    public class SearchDataMigration : DataMigrationImpl {

        public int Create() {

            ContentDefinitionManager.AlterTypeDefinition("SearchForm",
                cfg => cfg
                    .WithPart("SearchFormPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 2;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("SearchSettingsPartRecord", table => table
                .AddColumn<string>("SearchIndex", c => c.WithDefault("Search"))
            );

            return 2;
        }
    }
}