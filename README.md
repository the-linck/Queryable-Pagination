# Queryable-Pagination

## Why use this project?

Paginating queries may be a repetitive process. Even being an important thing to any well coded application, repetition may lead to errors and lack of standardization on the code.

To solve this, I've made this simple project that provide a simple - yet flexible - implementation of a strucure for pagination, that allows some pre-defined data formats and is ready to be extended as needed,

## IPaged

This interface is the core of the project, providing the required fields to deal with pagination without needing to access to queried data - in fact, to touching it at all. 

A pratical use of this interface is making a "generic" pagination partial view in a MVC project, that can be called on any result page.

It contains the following read-only properties:

* _int_ **Page**  
Current page number.
* _int_ **PageCount**  
Number of result pages
* _int_ **PerPage**  
Number of result pages
* _int_ **Total**  
Number of records avaliable to read.

## Implementations

All the 5 default implementations of IPaged are based on the generic class **Pagination&lt;*TQueryable*, *TResult*&gt;** - whose parameters represent, respectively, the data-type of entities that will be queried and the type of collection in wich this data will be stored.

Those are the provided implementations of IPaged:

* __PagedArray__ *&lt;TQueryable&gt;*  
Stores the queried data in an array of *TQueryable*.
* __PagedDictionary__ *&lt;TKey, TQueryable&gt;*  
Stores the queried data in a Dictionary of *TQueryable* with *TKey* keys.
* __PagedHashSet__ *&lt;TQueryable&gt;*  
Stores the queried data in a HashSet of *TQueryable*.
* __PagedLookup__ *&lt;TQueryable&gt;*  
Stores the queried data in a Lookup of *TQueryable* with *TKey* keys.
* __PagedList__ *&lt;TQueryable&gt;*  
Stores the queried data in a List of *TQueryable*.

## Extending

### Pagination class
All previous implementations derive from **Pagination** class, if you wish to make your own custom pagination class you must understand **Pagination&lt;*TQueryable*, *TResult*&gt;** first. If you don't need to make a custom implementation, feel free to skip this section.  
This class is a basic implementation of **IPaged** interface, providing 3 new properties:

* protected *TResult* **Data** *[read/write]*  
The Data that will be loaded by the query, already on the desired format.  
Internal use.
* public  *TResult* **Result** *[read only]*  
Readonly access to the queryied data.
* public  *int* **ToSkip** *[read only]*  
Number of records to skip from the begining.  
Internal use.

Along with them, a simple constructor is provided, wich reads the total number of registers from the *Query* and calculates the numeric properties for pagination (number of pages and rows ToSkip).  
It has the following parameters:

* *IOrderedQueryable&lt;TQueryable&gt;* **Query**  
The query to be paged, it must already be ordered (Entity Framework requirement for pagination).  
*It's type parameter is the same of the class.*
* *int* **Page** [Optional, default: 1]  
Current page of the results.
* *int* **PerPage** [Optional, default: 100]  
Number of records to read on each page.


### Custom types

You can easily make new implementations in the same way those above were made: 

* extend **Pagination&lt;*TQueryable*, *TResult*&gt;**,  
pass the type you will use as *TResult* class parameter
* create a constructor with the needed parameters  
*Query*, *Page* and *PerPage* are always recommended
* Call the base-constructor with *Query*, *Page* and *PerPage*
* Actually load the data from *Query*, using *.Skip(**ToSkip**)* and *.Take(**PerPage**)* for pagination.
* Store the converted data on the *Data* internal property  

## Extension Methods

For convenience, this project also provides extension methods for **IOrderedQueryable** that allows you to use pagination with a simple **toPaged{X}** call, being {X} the implementation of **IPaged** interface you want to use. 

For each **IPaged** implementation got it's extension method:

* *PagedArray&lt;T&gt;* **ToPagedArray**&lt;T&gt;(*int* Page = 1, *int* PerPage = 100)  
Creates an instance of Pagination that uses an array of *T* to store results.
* *PagedDictionary&lt;T, TKey&gt;* **ToPagedDictionary**&lt;T, TKey&gt;(*Func&lt;T, TKey&gt;* keySelector, *int* Page = 1, *int* PerPage = 100)  
Creates an instance of Pagination that uses a Dictionary of *T* with *TKey* keys to store results.
* *ToPagedHashSet&lt;T&gt;* **ToPagedHashSet**&lt;T&gt;(*int* Page = 1, *int* PerPage = 100)  
Creates an instance of Pagination that uses a HashSet of *T* to store results.
* *ToPagedLookup&lt;T, TKey&gt;* **ToPagedLookup**&lt;T, TKey&gt;(*Func&lt;T, TKey&gt;* keySelector, *int* Page = 1, *int* PerPage = 100)  
Creates an instance of Pagination that uses a Lookup of *T* with *TKey* keys to store results
* *ToPagedList&lt;T&gt;* **ToPagedList**&lt;T&gt;(*int* Page = 1, *int* PerPage = 100)  
Creates an instance of Pagination that uses a List of *T* to store results.



Using one of those methods is as easy as the following example, wich lists users that have the word "joker" in their e-mail address:

```c#
using(MyContext Context = new MyContext()) {
    Viewdata.model = Context.Users
        .Where(User => User.Email.ToLower().Contains("joker"))
        .OrderBy(User => User.Email)
    .ToPagedList(Request.Querystring["Page"] ?? "1");
}
```