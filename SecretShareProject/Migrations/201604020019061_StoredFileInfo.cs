namespace SecretShareProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoredFileInfo : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.FileInfoModels", newName: "FileInfoes");
            CreateTable(
                "dbo.StoredFileInfoes",
                c => new
                    {
                        storedId = c.Int(nullable: false, identity: true),
                        storageService = c.String(),
                        storedName = c.String(),
                        file_fileId = c.Int(),
                    })
                .PrimaryKey(t => t.storedId)
                .ForeignKey("dbo.FileInfoes", t => t.file_fileId)
                .Index(t => t.file_fileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoredFileInfoes", "file_fileId", "dbo.FileInfoes");
            DropIndex("dbo.StoredFileInfoes", new[] { "file_fileId" });
            DropTable("dbo.StoredFileInfoes");
            RenameTable(name: "dbo.FileInfoes", newName: "FileInfoModels");
        }
    }
}
