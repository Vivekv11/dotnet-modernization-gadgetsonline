using GadgetsOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetsOnline.Migrations
{
    public class Configuration : IEntityTypeConfiguration<GadgetsOnlineEntities>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<GadgetsOnlineEntities> builder)
        {
            // Configuration settings here
        }
    }
}
