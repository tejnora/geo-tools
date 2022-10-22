#ifndef at_exception_h_
#define at_exception_h_


namespace all_tran {

struct at_exception
{   	
	const char* location;
	const char* description;
	at_exception(const char* location,const char* description) : location(location),description(description) {}	
};

struct at_exception_gmv
{    
	GNU_gama::Exception::matvec& ex;
	const char* location;
	const char* description;
	at_exception_gmv(const char* location, const char* description, GNU_gama::Exception::matvec& exc) : location(location),description(description),ex(exc) {}	
};



} // namespace all_tran

#endif