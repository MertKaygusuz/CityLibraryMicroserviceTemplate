﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookServiceApi.Resources {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class BookValidationResource_tr_TR {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BookValidationResource_tr_TR() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("BookServiceApi.Resources.BookValidationResource_tr_TR", typeof(BookValidationResource_tr_TR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string Author_Required {
            get {
                return ResourceManager.GetString("Author_Required", resourceCulture);
            }
        }
        
        internal static string Book_Title_Required {
            get {
                return ResourceManager.GetString("Book_Title_Required", resourceCulture);
            }
        }
        
        internal static string First_Publish_Date_Required {
            get {
                return ResourceManager.GetString("First_Publish_Date_Required", resourceCulture);
            }
        }
        
        internal static string Edition_Number_Required {
            get {
                return ResourceManager.GetString("Edition_Number_Required", resourceCulture);
            }
        }
        
        internal static string Edition_Date_Required {
            get {
                return ResourceManager.GetString("Edition_Date_Required", resourceCulture);
            }
        }
        
        internal static string Available_Count_Required {
            get {
                return ResourceManager.GetString("Available_Count_Required", resourceCulture);
            }
        }
        
        internal static string Cover_Type_Invalid {
            get {
                return ResourceManager.GetString("Cover_Type_Invalid", resourceCulture);
            }
        }
        
        internal static string Cover_Type_Required {
            get {
                return ResourceManager.GetString("Cover_Type_Required", resourceCulture);
            }
        }
        
        internal static string Title_Type_Invalid {
            get {
                return ResourceManager.GetString("Title_Type_Invalid", resourceCulture);
            }
        }
        
        internal static string Title_Type_Required {
            get {
                return ResourceManager.GetString("Title_Type_Required", resourceCulture);
            }
        }
        
        internal static string Available_Count_Cannot_be_Negative {
            get {
                return ResourceManager.GetString("Available_Count_Cannot_be_Negative", resourceCulture);
            }
        }
        
        internal static string Edition_Number_Must_be_Positive {
            get {
                return ResourceManager.GetString("Edition_Number_Must_be_Positive", resourceCulture);
            }
        }
    }
}
