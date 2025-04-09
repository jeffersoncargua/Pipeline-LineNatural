using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineNatural.UnitTest.DTOs
{
    public class BaseTest 
    {
        //ValidationResult es una clase que permite obtener los resultados de una verificacion del DataAnnotation
        /// <summary>
        /// Esta funcion permite validar las decoraciones de DataAnnotation para comprobar que se cumplan con las condiciones establecidas en los DTO´s
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Retorna una lista con los errores que se cometieron al realizar las validaciones del DataAnnotation</returns>
        public List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null); //permite crear una instancia que permitira validar los parametros ingresados con el DataAnnotation
            Validator.TryValidateObject(model, ctx, validationResults, true);//Validator se puede usar para verificar que los parametros sean validos.
                                                                             //TryValidObject permite verificar que el modelo con los parametros ingresados son validos 
                                                                             //con las decoraciones de DataAnnotation y retorna la lista con los resultados obtenidos

            return validationResults;
        }
    }
}
