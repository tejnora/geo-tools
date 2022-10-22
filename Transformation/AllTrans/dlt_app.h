#ifndef all_tran_dlt_app_h_
#define all_tran_dlt_app_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class dlt_app : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from "xi" to "Xi" and the symbol of spatial coordinates vice versa

public:
dlt_app(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,11)  
        {tt="dlt_app";}

dlt_app(const Vec<> hh) : all_tran_base(hh,11)  
        {tt="dlt_app";}

dlt_app() : all_tran_base(11)  
        {tt="dlt_app";}

virtual ~dlt_app (){}

ostream& report_constants(ostream& out) const                           //virtual
	{
		out<<endl<<"The standard elements of camera inner and outer orientation are: f, x0, y0, p, m, lambda, X0, Y0, Z0, R";		
		Mat<> a(3,1),b(3,1),c(3,1),R(3,3),hu(3,1),ha(3,3);		
		for(int i = 1; i<=3; i++)	
		{
			a(i,1) = h(i);
			b(i,1) = h(4+i);
			c(i,1) = h(8+i);
		}
		double f,x0,y0,p,m,lambda,X0,Y0,Z0;
		x0 = (trans(a)*c)(1,1)/(trans(c)*c)(1,1);
		y0 = (trans(b)*c)(1,1)/(trans(c)*c)(1,1);
		f = sqrt( (trans(a)*a)(1,1)/(trans(c)*c)(1,1)- x0*x0 ) ;
		p = sqrt((trans(c)*c)(1,1));
		for(int i = 1; i<=3; i++)	
		{
			R(i,1) = a(i,1); R(i,2) = b(i,1); R(i,3) = c(i,1);
		}
		m = (-1)*determinant(R)/(p*p*p*f*f);
		lambda = ( ((trans(a)*b)*(trans(c)*c))(1,1) - ((trans(a)*c)*(trans(b)*c))(1,1) ) / ( ((trans(a)*a)*(trans(c)*c))(1,1) - ((trans(a)*c)*(trans(a)*c))(1,1) );
		hu(1,1) = -h(4); hu(2,1) = -h(8); hu(3,1) = -1;		
		R.transpose();
		hu = inv(R)* hu;
		X0 = hu(1,1); Y0 = hu(2,1); Z0 = hu(3,1);
		ha(1,1) = m; ha(1,2) = 0; ha(1,3) = -m*x0;
		ha(2,1) = -lambda; ha(2,2) = 1; ha(2,3) = lambda*x0-m*y0;
		ha(3,1) = 0; ha(3,2) = 0; ha(3,3) = -m*f;
		R = 1/(p*m*f)*ha*R;
		out<<endl<<"f = "<<f;
		out<<endl<<"x0 = "<<x0;
		out<<endl<<"y0 = "<<y0;
		out<<endl<<"p = "<<p;
		out<<endl<<"m = "<<m;
		out<<endl<<"lambda = "<<lambda;
		out<<endl<<"X0 = "<<X0;
		out<<endl<<"Y0 = "<<Y0;
		out<<endl<<"Z0 = "<<Z0;
		out<<endl<<"R: "<<R;
			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = X*L1 + Y*L2 + Z*L3 + L4 / (X*L9 + Y*L10 + Z*L11 + 1)";
		out<<endl<<"y = X*L5 + Y*L6 + Z*L7 + L8 / (X*L9 + Y*L10 + Z*L11 + 1)";
        out<<endl<<"Computed parameters are (in order L1, L2, ..., L11)";
        return out;
	}

protected:

void ApproxSolution() 
{
	h.set_zero();
	//not necessary because of linear form
}

/*
void fill_matrixes_dlt_app()
{
	if(nofc > 1)
	{		
		for(int i = 1; i <= r1; i++)
		{	
			X0T(i,1) = (*Xi)(i,1);
			X0T(i,2) = (*Xi)(i,2);
			
			kout<<endl;
			for(int j = 1; j <= 1; j++)
			{
				kout<<endl<<" X0T(i,1) ... : "<<X0T(i,1)<<' '<<X0T(i,2);
                X0T(i,1) = ((*xi)(i,1)*h(1) + (*xi)(i,2)*h(2) + (*xi)(i,3)*h(3) + h(4)) / ( (*xi)(i,1)*h(9) + (*xi)(i,2)*h(10) + (*xi)(i,3)*h(11) + 1);			
				X0T(i,2) = ((*xi)(i,1)*h(5) + (*xi)(i,2)*h(6) + (*xi)(i,3)*h(7) + h(8)) / ( (*xi)(i,1)*h(9) + (*xi)(i,2)*h(10) + (*xi)(i,3)*h(11) + 1);	
				kout<<endl<<" X0T(i,1) ... : "<<X0T(i,1)<<' '<<X0T(i,2);
				X0T(i,1) = ((*xi)(i,1)*h(1) + (*xi)(i,2)*h(2) + (*xi)(i,3)*h(3) + h(4)) - ( (*xi)(i,1)*(*Xi)(i,1)*h(9) + (*xi)(i,2)*(*Xi)(i,1)*h(10) + (*xi)(i,3)*(*Xi)(i,1)*h(11) );			
				X0T(i,2) = ((*xi)(i,1)*h(5) + (*xi)(i,2)*h(6) + (*xi)(i,3)*h(7) + h(8)) - ( (*xi)(i,1)*(*Xi)(i,2)*h(9) + (*xi)(i,2)*(*Xi)(i,2)*h(10) + (*xi)(i,3)*(*Xi)(i,2)*h(11) );	
				kout<<endl<<" X0T(i,1) ... : "<<X0T(i,1)<<' '<<X0T(i,2);

				//double a = ((*xi)(i,1)*h(1) + (*xi)(i,2)*h(2) + (*xi)(i,3)*h(3) + h(4)) - ( (*xi)(i,1)*X0T(i,1)*h(9) + (*xi)(i,2)*X0T(i,1)*h(10) + (*xi)(i,3)*X0T(i,1)*h(11) );
				//X0T(i,1) += a/( (*xi)(i,1)*h(9) + (*xi)(i,2)*h(10) + (*xi)(i,3)*h(11) ) ;
				//double b = ((*xi)(i,1)*h(5) + (*xi)(i,2)*h(6) + (*xi)(i,3)*h(7) + h(8)) - ( (*xi)(i,1)*X0T(i,2)*h(9) + (*xi)(i,2)*X0T(i,2)*h(10) + (*xi)(i,3)*X0T(i,2)*h(11) );	
				//X0T(i,2) += b/ ( (*xi)(i,1)*h(9) + (*xi)(i,2)*h(10) + (*xi)(i,3)*h(11) );
			}
			
		}
		//Xiiter = X0T;		
	}
	else	
	{
		X0T = (*Xi);
		//Xiiter = (*Xi);
	}
	//J	
	for(int i = 0; i < r1; i++)
	{
		J(i*n+1,1)=(*xi)(i+1,1);J(i*n+1,2)=(*xi)(i+1,2);J(i*n+1,3)=(*xi)(i+1,3);J(i*n+1,4)=1;J(i*n+1,9)= -(*xi)(i+1,1)*(*Xi)(i+1,1);J(i*n+1,10)= -(*xi)(i+1,2)*(*Xi)(i+1,1);J(i*n+1,11)= -(*xi)(i+1,3)*(*Xi)(i+1,1);
		J(i*n+2,5)=(*xi)(i+1,1);J(i*n+2,6)=(*xi)(i+1,2);J(i*n+2,7)=(*xi)(i+1,3);J(i*n+2,8)=1;J(i*n+2,9)= -(*xi)(i+1,1)*(*Xi)(i+1,2);J(i*n+2,10)= -(*xi)(i+1,2)*(*Xi)(i+1,2);J(i*n+2,11)= -(*xi)(i+1,3)*(*Xi)(i+1,2);		
	}
	//X0T
	if(nofc == 1)	X0T.set_zero();
}
*/


void fill_matrixes_dlt_app()
{
	if(nofc > 1)
	{		
		for(int i = 1; i <= r1; i++)
		{			
			X0T(i,1) = ((*xi)(i,1)*h(1) + (*xi)(i,2)*h(2) + (*xi)(i,3)*h(3) + h(4)) - ( (*xi)(i,1)*(*Xi)(i,1)*h(9) + (*xi)(i,2)*(*Xi)(i,1)*h(10) + (*xi)(i,3)*(*Xi)(i,1)*h(11) );			
			X0T(i,2) = ((*xi)(i,1)*h(5) + (*xi)(i,2)*h(6) + (*xi)(i,3)*h(7) + h(8)) - ( (*xi)(i,1)*(*Xi)(i,2)*h(9) + (*xi)(i,2)*(*Xi)(i,2)*h(10) + (*xi)(i,3)*(*Xi)(i,2)*h(11) );			
		}
		//Xiiter = X0T;		
	}
	else	
	{
		X0T.set_zero();		
	}
	//J	
	for(int i = 0; i < r1; i++)
	{
		J(i*n+1,1)=(*xi)(i+1,1);J(i*n+1,2)=(*xi)(i+1,2);J(i*n+1,3)=(*xi)(i+1,3);J(i*n+1,4)=1;J(i*n+1,9)= -(*xi)(i+1,1)*(*Xi)(i+1,1);J(i*n+1,10)= -(*xi)(i+1,2)*(*Xi)(i+1,1);J(i*n+1,11)= -(*xi)(i+1,3)*(*Xi)(i+1,1);
		J(i*n+2,5)=(*xi)(i+1,1);J(i*n+2,6)=(*xi)(i+1,2);J(i*n+2,7)=(*xi)(i+1,3);J(i*n+2,8)=1;J(i*n+2,9)= -(*xi)(i+1,1)*(*Xi)(i+1,2);J(i*n+2,10)= -(*xi)(i+1,2)*(*Xi)(i+1,2);J(i*n+2,11)= -(*xi)(i+1,3)*(*Xi)(i+1,2);		
	}	
}

void fill_matrixes() {fill_matrixes_dlt_app();}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,4);
		double hh;
		for(int i = 1; i <= num; i++)
		{
			hh = x(i,1)*h(9) + x(i,2)*h(10) + x(i,3)*h(11) + 1;
			X(i,1) = ( x(i,1)*h(1) + x(i,2)*h(2) + x(i,3)*h(3) + h(4))/hh;
			X(i,2) = ( x(i,1)*h(5) + x(i,2)*h(6) + x(i,3)*h(7) + h(8))/hh;			
		}	
		return X;
	}	
	else
	{
		throw at_exception("dlt::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace all_tran
#endif