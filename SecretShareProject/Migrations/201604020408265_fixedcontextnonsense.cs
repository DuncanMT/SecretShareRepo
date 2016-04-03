namespace SecretShareProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedcontextnonsense : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShareModels", "file_fileId", "dbo.FileInfoModels");
            DropIndex("dbo.ShareModels", new[] { "file_fileId" });
            RenameColumn(table: "dbo.ShareModels", name: "file_fileId", newName: "fileId");
            RenameColumn(table: "dbo.FileInfoModels", name: "User_Id", newName: "userID");
            RenameIndex(table: "dbo.FileInfoModels", name: "IX_User_Id", newName: "IX_userID");
            DropPrimaryKey("dbo.FileInfoModels");
            DropPrimaryKey("dbo.ShareModels");
            AddColumn("dbo.FileInfoModels", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ShareModels", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ShareModels", "fileId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.FileInfoModels", "Id");
            AddPrimaryKey("dbo.ShareModels", "Id");
            CreateIndex("dbo.ShareModels", "fileId");
            AddForeignKey("dbo.ShareModels", "fileId", "dbo.FileInfoModels", "Id");
            DropColumn("dbo.FileInfoModels", "fileId");
            DropColumn("dbo.ShareModels", "ShareId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShareModels", "ShareId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.FileInfoModels", "fileId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.ShareModels", "fileId", "dbo.FileInfoModels");
            DropIndex("dbo.ShareModels", new[] { "fileId" });
            DropPrimaryKey("dbo.ShareModels");
            DropPrimaryKey("dbo.FileInfoModels");
            AlterColumn("dbo.ShareModels", "fileId", c => c.Int());
            DropColumn("dbo.ShareModels", "Id");
            DropColumn("dbo.FileInfoModels", "Id");
            AddPrimaryKey("dbo.ShareModels", "ShareId");
            AddPrimaryKey("dbo.FileInfoModels", "fileId");
            RenameIndex(table: "dbo.FileInfoModels", name: "IX_userID", newName: "IX_User_Id");
            RenameColumn(table: "dbo.FileInfoModels", name: "userID", newName: "User_Id");
            RenameColumn(table: "dbo.ShareModels", name: "fileId", newName: "file_fileId");
            CreateIndex("dbo.ShareModels", "file_fileId");
            AddForeignKey("dbo.ShareModels", "file_fileId", "dbo.FileInfoModels", "fileId");
        }
    }
}
