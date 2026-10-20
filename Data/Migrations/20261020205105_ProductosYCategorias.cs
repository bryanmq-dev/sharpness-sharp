using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sharpness_sharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductosYCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    UrlImagen = table.Column<string>(type: "TEXT", nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Cafe" },
                    { 2, "Bebidas" },
                    { 3, "Panaderia" },
                    { 4, "Postres" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "CategoriaId", "Nombre", "Precio", "Stock", "UrlImagen" },
                values: new object[,]
                {
                    { 1, 1, "Americano", 12m, 100, "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd" },
                    { 2, 1, "Capuccino", 16m, 90, "https://images.unsplash.com/photo-1572442388796-11668a67e53d" },
                    { 3, 1, "Latte", 18m, 80, "https://images.unsplash.com/photo-1541167760496-1628856ab772" },
                    { 4, 1, "Flat White", 17m, 60, "https://images.unsplash.com/photo-1517256064527-09c73fc73e38" },
                    { 5, 1, "Cafe Filtrado", 15m, 40, "https://images.unsplash.com/photo-1442512595331-e89e73853f31" },
                    { 6, 2, "Cafe Helado", 18m, 50, "https://images.unsplash.com/photo-1461023058943-07fcbe16d735" },
                    { 7, 2, "Te Helado", 14m, 45, "https://images.unsplash.com/photo-1556679343-c7306c1976bc" },
                    { 8, 2, "Te con Leche", 13m, 35, "https://images.unsplash.com/photo-1544787219-7f47ccb76574" },
                    { 9, 3, "Croissant", 12m, 60, "https://images.unsplash.com/photo-1555507036-ab1f4038808a" },
                    { 10, 3, "Donas", 15m, 80, "https://images.unsplash.com/photo-1551024506-0bccd828d307" },
                    { 11, 3, "Galletas con Chips", 8m, 120, "https://images.unsplash.com/photo-1558961363-fa8fdf82db35" },
                    { 12, 4, "Pastel de Chocolate", 45m, 25, "https://images.unsplash.com/photo-1578985545062-69928b1d9587" },
                    { 13, 4, "Brownie", 14m, 55, "https://images.unsplash.com/photo-1571115177098-24ec42ed204d" },
                    { 14, 4, "Cupcakes", 18m, 70, "https://images.unsplash.com/photo-1495147466023-ac5c588e2e94" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
