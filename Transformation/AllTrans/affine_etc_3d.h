#ifndef all_tran_affine_3d_h_
#define all_tran_affine_3d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class affine_3d : public general_affine_m  
{
public:
affine_3d(const Mat<>& XXi,const Mat<>& xxi) : general_affine_m(XXi,xxi)
        {tt="affine_3d";q=6;}

affine_3d(const Vec<> hh) : general_affine_m(hh,3,15)  
        {tt="affine_3d";q=6;}

affine_3d() : general_affine_m(3,15)  
        {tt="affine_3d";q=6;}

virtual ~affine_3d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:

//void ApproxSolution(){}  -define in general_affine_m

void fill_matrixes_affine_3d()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T
	b(n+1) = (- R(1,1)*R(2,1) - R(1,2)*R(2,2) - R(1,3)*R(2,3) );
	b(n+2) = (- R(1,1)*R(3,1) - R(1,2)*R(3,2) - R(1,3)*R(3,3) );
	b(n+3) = (- R(3,1)*R(2,1) - R(3,2)*R(2,2) - R(3,3)*R(2,3) );	
	//J
	B(7,n+1) = R(2,1);
	B(8,n+1) = R(2,2);
	B(9,n+1) = R(2,3);
	B(10,n+1) = R(1,1);
	B(11,n+1) = R(1,2);
	B(12,n+1) = R(1,3);
	
	B(7,n+2) = R(3,1);
	B(8,n+2) = R(3,2);
	B(9,n+2) = R(3,3);
	B(13,n+2) = R(1,1);
	B(14,n+2) = R(1,2);
	B(15,n+2) = R(1,3);

	B(10,n+3) = R(3,1);
	B(11,n+3) = R(3,2);
	B(12,n+3) = R(3,3);
	B(13,n+3) = R(2,1);
	B(14,n+3) = R(2,2);
	B(15,n+3) = R(2,3);
}

void fill_matrixes()
{	
	fill_matrixes_general_affine_m();
	fill_matrixes_affine_3d();
}

//void transform_points(){}	-define in general_affine_m
			
};

class similarity_3d : public affine_3d  
{
public:
similarity_3d(const Mat<>& XXi,const Mat<>& xxi) : affine_3d(XXi,xxi)
        {tt="similarity_3d";q=8;}

similarity_3d(const Vec<> hh) : affine_3d(hh)  
	{tt="similarity_3d";q=8;}

similarity_3d() : affine_3d()  
	{tt="similarity_3d";q=8;}

virtual ~similarity_3d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:

//void ApproxSolution(){}  -define in general_affine_m

void fill_matrixes_similarity_3d()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T	
	b(n+4) = -h(4)+h(5);	
	b(n+5) = -h(5)+h(6);
	//J
	B(4,n+4) = 1;
	B(5,n+4) = -1;
		
	B(5,n+5) = 1;
	B(6,n+5) = -1;	
}

void fill_matrixes()
{	
	fill_matrixes_general_affine_m();
	fill_matrixes_affine_3d();
	fill_matrixes_similarity_3d();
}

//void transform_points(){}	-define in general_affine_m_
			
};

class identity_3d : public affine_3d  
{
public:
identity_3d(const Mat<>& XXi,const Mat<>& xxi) : affine_3d(XXi,xxi)
        {tt="identity_3d";q=9;}

identity_3d(const Vec<> hh) : affine_3d(hh)  
		{tt="identity_3d";q=9;}

identity_3d() : affine_3d()  
        {tt="identity_3d";q=9;}

virtual ~identity_3d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:

//void ApproxSolution(){}  -define in general_affine_m

void fill_matrixes_identity_3d()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T	
	b(n+4) = -h(4)+1;	
	b(n+5) = -h(5)+1;
	b(n+6) = -h(6)+1;
	//J
	B(4,n+4) = 1;
	B(5,n+5) = 1;		
	B(6,n+6) = 1;		
}

void fill_matrixes()
{	
	fill_matrixes_general_affine_m();
	fill_matrixes_affine_3d();
	fill_matrixes_identity_3d();
}

//void transform_points(){}	-define in general_affine_m
			
};


}   // namespace all_tran
#endif