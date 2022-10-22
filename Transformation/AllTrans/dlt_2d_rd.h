#ifndef all_tran_dlt_2d_rd_h_
#define all_tran_dlt_2d_rd_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class dlt_2d_rd : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
dlt_2d_rd(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,11)  
        {tt="dlt_2d_rd"; x0=v(1); y0=v(2); c = 2;}

dlt_2d_rd(const Vec<> hh, const Vec<> v) : all_tran_base(hh,11)  
        {tt="dlt_2d_rd"; x0=v(1); y0=v(2); c = 2;}

dlt_2d_rd() : all_tran_base(11)  
        {tt="dlt_2d_rd"; c = 2;}

void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v)  //virtual
		{
			x0=v(1); y0=v(2);
			all_tran_base::reset(XXi,xxi);
		}

void reset(const Vec<> hh,const Vec<> v)  //virtual
		{
			x0=v(1); y0=v(2);
			all_tran_base::reset(hh);
		}

virtual ~dlt_2d_rd (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = X*L1 + Y*L2 + L3 / (X*L7 + Y*L8 + 1) - (x-x0)*(k1*r^2+k2*r^4+k3*r^6)";
		out<<endl<<"y = X*L4 + Y*L5 + L6 / (X*L7 + Y*L8 + 1) - (y-y0)*(k1*r^2+k2*r^4+k3*r^6)";
        out<<endl<<"Computed parameters are (in order L1, L2, ..., L8, k1, k2, k3)";
        return out;
	}

protected:
double x0,y0;

void ApproxSolution() 
{
	h.set_zero();
	dlt_2d at((*Xi),(*xi));
	at.solve();
	Vec<> hh;
	hh = at.get_solution();	
	for(int i = 1; i <= 8; i++)	h(i) = hh(i);	
}

void fill_matrixes_dlt_2d_rd()
{	
	//J a X0T			
	double A,B,C,r,rr,xx,yy;
	for(int i = 1; i <= r1; i++)
	{			
		A = (*xi)(i,1)*h(1) + (*xi)(i,2)*h(2) + h(3);
		B = (*xi)(i,1)*h(4) + (*xi)(i,2)*h(5) + h(6);
		C = (*xi)(i,1)*h(7) + (*xi)(i,2)*h(8) + 1;	

		xx = (*Xi)(i,1)-x0;
		yy = (*Xi)(i,2)-y0;
		r = sqrt( xx*xx + yy*yy );	
		rr = h(9)*r*r + h(10)*r*r*r*r + h(11)*r*r*r*r*r*r;			
		X0T(i,1) = A/C - rr * xx;
		X0T(i,2) = B/C - rr * yy;			
		
		J((i-1)*n+1,1) = (*xi)(i,1)/C; J((i-1)*n+1,2)=(*xi)(i,2)/C; J((i-1)*n+1,3)=1/C;
		J((i-1)*n+1,7) = -A*(*xi)(i,1)/(C*C); J((i-1)*n+1,8)= -A*(*xi)(i,2)/(C*C);
		J((i-1)*n+1,9)= -xx*r*r; J((i-1)*n+1,10)= -xx*r*r*r*r; J((i-1)*n+1,11)= -xx*r*r*r*r*r*r;

		J((i-1)*n+2,4)=(*xi)(i,1)/C; J((i-1)*n+2,5)=(*xi)(i,2)/C; J((i-1)*n+2,6)=1/C;
		J((i-1)*n+2,7)= -B*(*xi)(i,1)/(C*C); J((i-1)*n+2,8)= -B*(*xi)(i,2)/(C*C);
		J((i-1)*n+2,9)= -yy*r*r; J((i-1)*n+2,10)= -yy*r*r*r*r; J((i-1)*n+2,11)= -yy*r*r*r*r*r*r;		
	}	
}

void fill_matrixes() {fill_matrixes_dlt_2d_rd();}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);	
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);
		
		double A,B,C,r,rr,xx,yy;
		for(int i = 1; i <= num; i++)
		{
			A = x(i,1)*h(1) + x(i,2)*h(2) + h(3);
			B = x(i,1)*h(4) + x(i,2)*h(5) + h(6);
			C = x(i,1)*h(7) + x(i,2)*h(8) + 1;
			X(i,1) = A/C;
			X(i,2) = B/C;
			for(int j = 1; j <= 4; j++)
			{
				xx = (X(i,1)-x0);
				yy = (X(i,2)-y0);
				r = sqrt( xx*xx + yy*yy );				
				rr = h(9)*r*r + h(10)*r*r*r*r + h(11)*r*r*r*r*r*r;				
				X(i,1) = A/C - rr * (X(i,1)-x0);
				X(i,2) = B/C - rr * (X(i,2)-y0);
				//kout<<'\n'<<"iter: "<<j<<' '<<X(i,1)<<' '<<X(i,2)<<'\n';
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("dlt_2d_rd::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace spat_fig
#endif