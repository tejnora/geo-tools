#ifndef all_tran_rd_td_h_
#define all_tran_rd_td_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class rd_td : public all_tran_base  
{
public:
rd_td(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,5)  
        {tt="rd_td";q=0;c=2; x0=v(1); y0=v(2);}

rd_td(const Vec<> hh, const Vec<> v) : all_tran_base(hh,5)  
        {tt="rd_td"; x0=v(1); y0=v(2); c = 2;}

//for the case of composite transformation
rd_td() : all_tran_base(5)	{tt = "rd_td";c = 2;}

void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v)
{
	x0=v(1); y0=v(2);
	all_tran_base::reset(XXi,xxi);
}
//for the case of composite transformation


virtual ~rd_td (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"X = x-(X-x0)*(k1*r^2+k2*r^4+k3*r^6)-p1*(r^2+2*(X-x0)^2)-2*p2*(X-x0)*(Y-y0)";
		out<<endl<<"Y = y-(Y-y0)*(k1*r^2+k2*r^4+k3*r^6)-p2*(r^2+2*(Y-y0)^2)-2*p1*(X-x0)*(Y-y0)";
        out<<endl<<"Computed parameters are (in order k1, k2, k3, p1, p2)";
        return out;
	}

protected:
	double x0,y0;

void ApproxSolution() 
{
	h.set_zero();		
}

void fill_matrixes_rd_td()
{	
	//J a X0T			
	double r,rr,x,y;
	for(int i = 0; i < r1; i++)
	{
		x = (*Xi)(i+1,1)-x0;
		y = (*Xi)(i+1,2)-y0;
		r = sqrt( x*x + y*y );
		rr = h(1)*r*r + h(2)*r*r*r*r + h(3)*r*r*r*r*r*r;				
				
		J(i*n+1,1)= -x*r*r;
		J(i*n+1,2)= -x*r*r*r*r;
		J(i*n+1,3)= -x*r*r*r*r*r*r;
		J(i*n+1,4)= -(r*r+2*x*x);
		J(i*n+1,5)= -2*x*y;
		
		J(i*n+2,1)= -y*r*r;
		J(i*n+2,2)= -y*r*r*r*r;
		J(i*n+2,3)= -y*r*r*r*r*r*r;
		J(i*n+2,4)= -2*x*y;
		J(i*n+2,5)= -(r*r+2*y*y);
				
		X0T(i+1,1) = (*xi)(i+1,1) - x * rr - h(4)*(r*r+2*x*x) - h(5)*2*x*y ;
		X0T(i+1,2) = (*xi)(i+1,2) - y * rr - h(5)*(r*r+2*y*y) - h(4)*2*x*y ;
	}	
}

void fill_matrixes() {fill_matrixes_rd_td();}

public:
Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);
		
		double r,rr,xx,yy;
		for(int i = 1; i <= num; i++)
		{
			X(i,1) = x(i,1);
			X(i,2) = x(i,2);
			for(int j = 1; j <= 4; j++)
			{
				xx = X(i,1)-x0;
				yy = X(i,2)-y0;
				r = sqrt( xx*xx + yy*yy );
				rr = h(1)*r*r + h(2)*r*r*r*r + h(3)*r*r*r*r*r*r;	
						
				X(i,1) = x(i,1) - xx * rr - h(4)*(r*r+2*xx*xx) - h(5)*2*xx*yy ;
				X(i,2) = x(i,2) - yy * rr - h(5)*(r*r+2*yy*yy) - h(4)*2*xx*yy ;
				//kout<<'\n'<<"iter: "<<j<<' '<<X(i,1)<<' '<<X(i,2)<<'\n';
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("rd_td::transform_points","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}

Mat<> transform_points_inversion(const Mat<>& X)
{
	if(is_solved)
	{
		int num = X.rows();
		Mat<> x(num,3);		
		double r,rr,xx,yy;		
		for(int i = 1; i <= num; i++)	
		{
			x(i,3) = X(i,3);
			xx = X(i,1)-x0;
			yy = X(i,2)-y0;
			r = sqrt( xx*xx + yy*yy );
			rr = h(1)*r*r + h(2)*r*r*r*r + h(3)*r*r*r*r*r*r;
			x(i,1) = X(i,1) + xx * rr + h(4)*(r*r+2*xx*xx) + h(5)*2*xx*yy ;
			x(i,2) = X(i,2) + yy * rr + h(5)*(r*r+2*yy*yy) + h(4)*2*xx*yy ;			
		}
		return x;
	}	
	else
	{
		throw at_exception("rd_td::transform_points_inversion","Transformation key is not solved yet. Call method \"solve()\" first");
	}		
}			
};

}   // namespace spat_fig
#endif