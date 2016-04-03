namespace SecretShareProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class filesize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileInfoModels", "fileSize", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileInfoModels", "fileSize");
        }
    }
}
