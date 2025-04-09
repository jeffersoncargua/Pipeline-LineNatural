using LineNatural.Context;
using LineNatural.SharedDatabaseSetup;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace LineNatural.IntegratedTest
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object _lock = new object(); //permite generar un objeto generico para poder bloquear el acceso cuando
                                                  //se realicen peticiones simultaneas y necesiten realizas operaciones que muten la base de datos
        private bool _databaseInitialized; //Permite establecer cuando se ha iniciado las transacciones con la base de datos

        private readonly string dbName = "IntegratedTest.db";//Es el nombre de la base de datos simulada en SQLite
        public SharedDatabaseFixture()
        {
            Connection = new SqliteConnection($"Filename={dbName}"); //permite instanciar la conexion de la base de datos simulada
            Seed();//Permite llenar de datos simulados a la base de datos a traves de bogus
            Connection.Open();//Permite conectar la base de datos con los repositorios test para realizar transacciones con la base de datos
        }
        public DbConnection Connection { get; } //permite instanciar una clase para conectar la base de datos simulada Sqlite
        /// <summary>
        /// Esta funcion permite realizar las transacciones en la base de datos simulada a traves de ApplicationDbcontext para utilizar
        /// Add, Update, Remove
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>Devuelve el context para utilizar la informacion simulada en la base de datos</returns>
        public ApplicationDbContext CreateContext(DbTransaction? transaction = null)
        {
            var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }
            return context;
        }
        /// <summary>
        /// Permite agregar datos simulados para emplearlos en la base de datos 
        /// </summary>
        private void Seed()
        {
            //lock permite bloquear las transacciones hasta que se haya ejecutado el bloque de codigo que se haya solicitado primero
            //Y se desbloqea una vez que se haya culminado con la ejecucion del codigo que para este caso es la de llenar la base de 
            //datos son la infomracion simulada en el proyecto LineNatural.SharedDatabaseSetup
            lock(_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        DatabaseSetup.SeedData(context);
                    }
                    _databaseInitialized = true;
                }
            }
        }
        //La funcion Dispose() permite desconectar la base de datos luego de que se haya realizado la transaccion 
        //EL proposito es de que se pueda utilizar por otros procesos que requieran la base de datos y necesiten conectarse
        public void Dispose() => Connection.Dispose();
    }
}
