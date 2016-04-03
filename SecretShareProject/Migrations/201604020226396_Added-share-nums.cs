namespace SecretShareProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedsharenums : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileInfoModels", "numshares", c => c.Int(nullable: false));
            AddColumn("dbo.FileInfoModels", "reqshares", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileInfoModels", "reqshares");
            DropColumn("dbo.FileInfoModels", "numshares");
        }
    }
}
