namespace SecretShareProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedtoshares : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.FileInfoes", newName: "FileInfoModels");
            DropForeignKey("dbo.StoredFileInfoes", "file_fileId", "dbo.FileInfoes");
            DropIndex("dbo.StoredFileInfoes", new[] { "file_fileId" });
            CreateTable(
                "dbo.ShareModels",
                c => new
                    {
                        ShareId = c.Int(nullable: false, identity: true),
                        storageService = c.String(),
                        shareName = c.String(),
                        file_fileId = c.Int(),
                    })
                .PrimaryKey(t => t.ShareId)
                .ForeignKey("dbo.FileInfoModels", t => t.file_fileId)
                .Index(t => t.file_fileId);
            
            DropTable("dbo.StoredFileInfoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StoredFileInfoes",
                c => new
                    {
                        storedId = c.Int(nullable: false, identity: true),
                        storageService = c.String(),
                        storedName = c.String(),
                        file_fileId = c.Int(),
                    })
                .PrimaryKey(t => t.storedId);
            
            DropForeignKey("dbo.ShareModels", "file_fileId", "dbo.FileInfoModels");
            DropIndex("dbo.ShareModels", new[] { "file_fileId" });
            DropTable("dbo.ShareModels");
            CreateIndex("dbo.StoredFileInfoes", "file_fileId");
            AddForeignKey("dbo.StoredFileInfoes", "file_fileId", "dbo.FileInfoes", "fileId");
            RenameTable(name: "dbo.FileInfoModels", newName: "FileInfoes");
        }
    }
}
