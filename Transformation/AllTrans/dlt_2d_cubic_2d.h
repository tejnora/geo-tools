#ifndef all_tran_dlt_2d_cubic_2d_h_
#define all_tran_dlt_2d_cubic_2d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------
double nn = 1;

class dlt_2d_cubic_2d : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
dlt_2d_cubic_2d(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,26)  
        {tt="dlt_2d_cubic_2d";}

dlt_2d_cubic_2d(const Vec<> hh) : all_tran_base(hh,26)  
        {tt="dlt_2d_cubic_2d";}

dlt_2d_cubic_2d() : all_tran_base(26)  
        {tt="dlt_2d_cubic_2d";}

void reset(const Mat<>& XXi,const Mat<>& xxi)  //virtual
		{			
			all_tran_base::reset(XXi,xxi);
		}

void reset(const Vec<> hh)  //virtual
		{			
			all_tran_base::reset(hh);
		}

virtual ~dlt_2d_cubic_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = X*L1 + Y*L2 + L3 / (X*L7 + Y*L8 + 1) - ( h1*x + h2*y + h3*x^2 + h4*xy + h5*y^2 + h11*x^3 + h12*x*y^2 + h13*x^2*y + h14*y^3 )";
		out<<endl<<"y = X*L4 + Y*L5 + L6 / (X*L7 + Y*L8 + 1) - ( h6*x + h7*y + h8*x^2 + h9*xy + h10*y^2 + h15*x^3 + h16*x*y^2 + h17*x^2*y + h18*y^3 )";
        out<<endl<<"Computed parameters are (in order h1, h2, ..., h18, L1, L2, ..., L8)";
        return out;
	}

protected:

void ApproxSolution() 
{
	h.set_zero();
	dlt_2d at((*Xi),(*xi));
	at.solve();
	Vec<> hh;
	hh = at.get_solution();	
	for(int i = 1; i <= 8; i++)	h(18+i) = hh(i);	
}

void fill_matrixes_dlt_2d_cubic_2d()
{	
	//J a X0T			
	double A,B,C;
	for(int i = 1; i <= r1; i++)
	{			
		A = (*xi)(i,1)*h(19) + (*xi)(i,2)*h(20) + h(21);
		B = (*xi)(i,1)*h(22) + (*xi)(i,2)*h(23) + h(24);
		C = (*xi)(i,1)*h(25) + (*xi)(i,2)*h(26) + 1;														
		X0T(i,1) = A/C;
		X0T(i,2) = B/C;					
		X0T(i,1) -= nn*h(1)*(*Xi)(i,1) + nn*h(2)*(*Xi)(i,2) + nn*h(3)*(*Xi)(i,1)*(*Xi)(i,1) + nn*h(4)*(*Xi)(i,1)*(*Xi)(i,2) + nn*h(5)*(*Xi)(i,2)*(*Xi)(i,2);
		X0T(i,2) -= nn*h(6)*(*Xi)(i,1) + nn*h(7)*(*Xi)(i,2) + nn*h(8)*(*Xi)(i,1)*(*Xi)(i,1) + nn*h(9)*(*Xi)(i,1)*(*Xi)(i,2) + nn*h(10)*(*Xi)(i,2)*(*Xi)(i,2);
		X0T(i,1) -= nn*h(11)*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,1) + nn*h(12)*(*Xi)(i,1)*(*Xi)(i,2)*(*Xi)(i,2) + nn*h(13)*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,2)
			+ nn*h(14)*(*Xi)(i,2)*(*Xi)(i,2)*(*Xi)(i,2);
		X0T(i,2) -= nn*h(15)*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,1) + nn*h(16)*(*Xi)(i,1)*(*Xi)(i,2)*(*Xi)(i,2) + nn*h(17)*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,2)
			+ nn*h(18)*(*Xi)(i,2)*(*Xi)(i,2)*(*Xi)(i,2);		
		
		J((i-1)*n+1,1) = -nn*(*Xi)(i,1); J((i-1)*n+1,2) = -nn*(*Xi)(i,2); J((i-1)*n+1,3) = -nn*(*Xi)(i,1)*(*Xi)(i,1); 
		J((i-1)*n+1,4)= -nn*(*Xi)(i,1)*(*Xi)(i,2); J((i-1)*n+1,5)= -nn*(*Xi)(i,2)*(*Xi)(i,2);
		J((i-1)*n+2,6) = -nn*(*Xi)(i,1); J((i-1)*n+2,7) = -nn*(*Xi)(i,2); J((i-1)*n+2,8) = -nn*(*Xi)(i,1)*(*Xi)(i,1); 
		J((i-1)*n+2,9)= -nn*(*Xi)(i,1)*(*Xi)(i,2); J((i-1)*n+2,10)= -nn*(*Xi)(i,2)*(*Xi)(i,2);

		J((i-1)*n+1,11) = -nn*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,1); J((i-1)*n+1,12) = -nn*(*Xi)(i,1)*(*Xi)(i,2)*(*Xi)(i,2); 
		J((i-1)*n+1,13) = -nn*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,2);	J((i-1)*n+1,14)= -nn*(*Xi)(i,2)*(*Xi)(i,2)*(*Xi)(i,2);
		J((i-1)*n+2,15) = -nn*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,1); J((i-1)*n+2,16) = -nn*(*Xi)(i,1)*(*Xi)(i,2)*(*Xi)(i,2); 
		J((i-1)*n+2,17) = -nn*(*Xi)(i,1)*(*Xi)(i,1)*(*Xi)(i,2);	J((i-1)*n+2,18)= -nn*(*Xi)(i,2)*(*Xi)(i,2)*(*Xi)(i,2);


		J((i-1)*n+1,19) = (*xi)(i,1)/C; J((i-1)*n+1,20)=(*xi)(i,2)/C; J((i-1)*n+1,21)=1/C;
		J((i-1)*n+1,25) = -A*(*xi)(i,1)/(C*C); J((i-1)*n+1,26)= -A*(*xi)(i,2)/(C*C);
		
		J((i-1)*n+2,22)=(*xi)(i,1)/C; J((i-1)*n+2,23)=(*xi)(i,2)/C; J((i-1)*n+2,24)=1/C;
		J((i-1)*n+2,25)= -B*(*xi)(i,1)/(C*C); J((i-1)*n+2,26)= -B*(*xi)(i,2)/(C*C);		
	}	
}

void fill_matrixes() {fill_matrixes_dlt_2d_cubic_2d();}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);	
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);
		
		double A,B,C,dx,dy;
		for(int i = 1; i <= num; i++)
		{
			A = x(i,1)*h(19) + x(i,2)*h(20) + h(21);
			B = x(i,1)*h(22) + x(i,2)*h(23) + h(24);
			C = x(i,1)*h(25) + x(i,2)*h(26) + 1;			
			X(i,1) = A/C;
			X(i,2) = B/C;
			for(int j = 1; j <= 3; j++)
			{					
				dx = nn*h(1)*X(i,1) + nn*h(2)*X(i,2) + nn*h(3)*X(i,1)*X(i,1) + nn*h(4)*X(i,1)*X(i,2) + nn*h(5)*X(i,2)*X(i,2) +
						nn*h(11)*X(i,1)*X(i,1)*X(i,1) + nn*h(12)*X(i,1)*X(i,2)*X(i,2) + nn*h(13)*X(i,1)*X(i,1)*X(i,2)
						+ nn*h(14)*X(i,2)*X(i,2)*X(i,2);
				dy = nn*h(6)*X(i,1) + nn*h(7)*X(i,2) + nn*h(8)*X(i,1)*X(i,1) + nn*h(9)*X(i,1)*X(i,2) + nn*h(10)*X(i,2)*X(i,2) +		
						nn*h(15)*X(i,1)*X(i,1)*X(i,1) + nn*h(16)*X(i,1)*X(i,2)*X(i,2) + nn*h(17)*X(i,1)*X(i,1)*X(i,2)
						+ nn*h(18)*X(i,2)*X(i,2)*X(i,2);
				X(i,1) = A/C - dx;
				X(i,2) = B/C - dy;
				//kout<<'\n'<<"i: "<<i<<' '<<X(i,1)<<' '<<X(i,2);
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("dlt_2d_cubic_2d::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace spat_fig
#endif