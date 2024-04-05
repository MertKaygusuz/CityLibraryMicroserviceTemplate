﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookServiceApi.Dtos.Book
{
    public class DeleteBookDto
    {
        //this annotations are required for GenericNotFoundFilter
        [Key]
        [DisplayName("Book")] 
        public int BookId { get; set; }
    }
}
