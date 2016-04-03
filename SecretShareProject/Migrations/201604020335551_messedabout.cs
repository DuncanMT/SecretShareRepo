namespace SecretShareProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messedabout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileInfoModels", "minshares", c => c.Int(nullable: false));
            DropColumn("dbo.FileInfoModels", "reqshares");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileInfoModels", "reqshares", c => c.Int(nullable: false));
            DropColumn("dbo.FileInfoModels", "minshares");
        }
    }
}
