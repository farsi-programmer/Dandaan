using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    [Table(Name = nameof(DandaanSetting))]
    public class DandaanSetting
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int DandaanUser { get; set; }

        [Column]
        public byte FormMainWindowState { get; set; }

        public const string CreateTable = @"
IF (SELECT count(*) FROM information_schema.tables WHERE table_name=N'DandaanSetting') < 1
BEGIN
    CREATE TABLE [dbo].[DandaanSetting](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [DandaanUser] [int] NOT NULL,
	    [FormMainWindowState] [tinyint] NOT NULL,
     CONSTRAINT [PK_DandaanSetting] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[DandaanSetting]  WITH CHECK ADD  CONSTRAINT [FK_DandaanSetting_DandaanUser] FOREIGN KEY([DandaanUser])
    REFERENCES [dbo].[DandaanUser] ([Id])

    ALTER TABLE [dbo].[DandaanSetting] CHECK CONSTRAINT [FK_DandaanSetting_DandaanUser] 
END";
        public static DandaanSetting Select(int dandaanUser)
        {
            using (var context = DB.LinqContext)
                return context.DandaanSettings.Where(s => s.DandaanUser == dandaanUser).FirstOrDefault();
        }

        public static void Insert(DandaanSetting dandaanSetting)
        {
            DB.ExecuteNonQuery($@"insert into 
DandaanSetting({nameof(DandaanUser)}, {nameof(FormMainWindowState)})
values(@{nameof(DandaanUser)}, @{nameof(FormMainWindowState)})",
                new SqlParameter($"@{nameof(DandaanUser)}", SqlDbType.Int)
                { Value = dandaanSetting.DandaanUser },
                new SqlParameter($"@{nameof(FormMainWindowState)}", SqlDbType.TinyInt)
                { Value = dandaanSetting.FormMainWindowState });
        }
    }
}
