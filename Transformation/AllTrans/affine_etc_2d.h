#ifndef all_tran_affine_2d_h_
#define all_tran_affine_2d_h_

#include "general_affine_multi.h"

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class affine_2d : public general_affine_m  
{
public:
affine_2d(const Mat<>& XXi,const Mat<>& xxi) : general_affine_m(XXi,xxi)
        {tt="affine_2d";q=3;}

affine_2d(const Vec<> hh) : general_affine_m(hh,2,8)  
        {tt="affine_2d";q=3;}

affine_2d() : general_affine_m(2,8)  
        {tt="affine_2d";q=3;}

virtual ~affine_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:

//void ApproxSolution(){}  -define in general_affine_m

void fill_matrixes_affine_2d()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T
	b(n+1) = (- R(1,1)*R(2,1) - R(1,2)*R(2,2) );	
	//J
	B(5,n+1) = R(2,1);
	B(6,n+1) = R(2,2);	
	B(7,n+1) = R(1,1);
	B(8,n+1) = R(1,2);	
}

void fill_matrixes()
{	
	fill_matrixes_general_affine_m();
	fill_matrixes_affine_2d();
}

//void transform_points(){}	-define in general_affine_m
			
};

class similarity_2d : public affine_2d  
{
public:
similarity_2d(const Mat<>& XXi,const Mat<>& xxi) : affine_2d(XXi,xxi)
        {tt="similarity_2d";q=4;}

similarity_2d(const Vec<> hh) : affine_2d(hh)  
        {tt="similarity_2d";q=4;}

similarity_2d() : affine_2d()  
        {tt="similarity_2d";q=4;}

virtual ~similarity_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:

//void ApproxSolution(){}  -define in general_affine_m

void fill_matrixes_similarity_2d()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T	
	b(n+2) = -h(3)+h(4);		
	//J
	B(3,n+2) = 1;
	B(4,n+2) = -1;	
}

void fill_matrixes()
{	
	fill_matrixes_general_affine_m();
	fill_matrixes_affine_2d();
	fill_matrixes_similarity_2d();
}

//void transform_points(){}	-define in general_affine_m_			
};

class identity_2d : public affine_2d  
{
public:
identity_2d(const Mat<>& XXi,const Mat<>& xxi) : affine_2d(XXi,xxi)
        {tt="identity_2d";q=5;}

identity_2d(const Vec<> hh) : affine_2d(hh)  
        {tt="identity_2d";q=5;}

identity_2d() : affine_2d()  
        {tt="identity_2d";q=5;}

virtual ~identity_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:

//void ApproxSolution(){}  -define in general_affine_m

void fill_matrixes_identity_2d()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T	
	b(n+2) = -h(3)+1;	
	b(n+3) = -h(4)+1;	
	//J
	B(3,n+2) = 1;
	B(4,n+3) = 1;				
}

void fill_matrixes()
{	
	fill_matrixes_general_affine_m();
	fill_matrixes_affine_2d();
	fill_matrixes_identity_2d();
}

//void transform_points(){}	-define in general_affine_m
			
};


}   // namespace spat_fig
#endif