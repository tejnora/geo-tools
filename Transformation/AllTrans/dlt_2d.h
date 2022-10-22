#ifndef all_tran_dlt_2d_h_
#define all_tran_dlt_2d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class dlt_2d : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
dlt_2d(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,8)  
        {tt="dlt_2d";}

dlt_2d(const Vec<> hh) : all_tran_base(hh,8)  
        {tt="dlt_2d";}

dlt_2d() : all_tran_base(8)  
        {tt="dlt_2d";}
//for the case of composite transformation

virtual ~dlt_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = X*L1 + Y*L2 + L3 / (X*L7 + Y*L8 + 1)";
		out<<endl<<"y = X*L4 + Y*L5 + L6 / (X*L7 + Y*L8 + 1)";
        out<<endl<<"Computed parameters are (in order L1, L2, ..., L8)";
        return out;
	}

protected:

void ApproxSolution() 
{
	dlt_2d_app at((*Xi),(*xi));
	at.solve();
	h = at.get_solution();	
}

void fill_matrixes_dlt_2d()
{		
	//J a X0T
	double A,B,C;
	for(int i = 0; i < r1; i++)
	{
		A = (*xi)(i+1,1)*h(1) + (*xi)(i+1,2)*h(2) + h(3);
		B = (*xi)(i+1,1)*h(4) + (*xi)(i+1,2)*h(5) + h(6);
		C = (*xi)(i+1,1)*h(7) + (*xi)(i+1,2)*h(8) + 1;

		J(i*n+1,1) = (*xi)(i+1,1)/C; J(i*n+1,2)=(*xi)(i+1,2)/C; J(i*n+1,3)=1/C;
		J(i*n+1,7) = -A*(*xi)(i+1,1)/(C*C); J(i*n+1,8)= -A*(*xi)(i+1,2)/(C*C);

		J(i*n+2,4)=(*xi)(i+1,1)/C; J(i*n+2,5)=(*xi)(i+1,2)/C; J(i*n+2,6)=1/C;
		J(i*n+2,7)= -B*(*xi)(i+1,1)/(C*C); J(i*n+2,8)= -B*(*xi)(i+1,2)/(C*C);
		
		X0T(i+1,1) = A/C;
		X0T(i+1,2) = B/C;
	}	
}

void fill_matrixes() {fill_matrixes_dlt_2d();}

public:
Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);
		double hh;
		for(int i = 1; i <= num; i++)
		{
			hh = x(i,1)*h(7) + x(i,2)*h(8) + 1;
			X(i,1) = ( x(i,1)*h(1) + x(i,2)*h(2) + h(3))/hh;
			X(i,2) = ( x(i,1)*h(4) + x(i,2)*h(5) + h(6))/hh;		
		}	
		return X;
	}	
	else
	{
		throw at_exception("dlt_2d::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace spat_fig
#endif