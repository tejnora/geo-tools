#ifndef all_tran_thin_plate_spline_2d_h_
#define all_tran_thin_plate_spline_2d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class thin_plate_spline_2d : public all_tran_base  
{
public:
thin_plate_spline_2d(const Mat<>& XXi,const Mat<>& xxi, const Vec<> v) : all_tran_base(XXi,xxi, 2*(2*xxi.rows()+3) )
        {tt="thin_plate_spline_2d"; c=1; lambda = v(1);}
		// == regularization parameter, if lambda == 0 coresponding points match exactly

thin_plate_spline_2d(const Vec<> hh, const Vec<> v) : all_tran_base(hh, hh.dim())
	{tt="thin_plate_spline_2d"; c=1; lambda = v(1);}

//because of unknown number of parameters the control after reset(h,v) isnt possible
thin_plate_spline_2d() : all_tran_base(1)  //
        {tt="thin_plate_spline_2d"; c = 1;}

void reset(const Vec<> hh,const Vec<> v)  //virtual
		{
			lambda = v(1);
			h = hh;
			is_solved = true;
		}

//in the case that number of parameters is known, there is a special constructor
thin_plate_spline_2d(int p) : all_tran_base(p)	{tt = "thin_plate_spline_2d"; c = 1;}          

void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v)
{
	p = 2*(2*xxi.rows()+3);
	lambda = v(1);
	all_tran_base::reset(XXi,xxi);
}

virtual ~thin_plate_spline_2d (){}

protected:
double lambda; // == regularization parameter, if lambda == 0 coresponding points match exactly

public:
virtual void solve()    
{
	Mat<> K(r1,r1);
	
	double rar;
    for(int i = 1; i <= r1; i++)
		for(int j = 1; j <= r1; j++)
		{
			rar = sqrt( ((*xi)(i,1)-(*xi)(j,1))*((*xi)(i,1)-(*xi)(j,1)) + ((*xi)(i,2)-(*xi)(j,2))*((*xi)(i,2)-(*xi)(j,2)) );
			if(i != j)
				K(i,j) = rar*rar*log(rar*rar);
			else K(i,j) = 0;
		}

	Mat<> P(r1,3);
	for(int i = 1; i <= r1; i++)
	{
		P(i,1) = 1;
		P(i,2) = (*xi)(i,1);
		P(i,3) = (*xi)(i,2);
	}	

	Mat<> L(r1+3,r1+3);
	L.set_zero();
	Mat<> PT = trans(P);
	for(int i = 1; i <= r1; i++)
		for(int j = 1; j <= r1; j++)	
		{
			//kout<<endl<<"i "<<i<<" j "<<j<<" lambda "<<lambda;
			if(i==j)	L(i,j) = lambda + K(i,j);
			else		L(i,j) = K(i,j);			
		}
	for(int i = 1; i <= r1; i++)
		for(int j = 1; j <= 3; j++)		
		{
			L(i,r1+j) = P(i,j);
			L(r1+j,i) = PT(j,i);
		}
	
	Mat<> Y(r1+3,2);
	Y.set_zero();
	for(int i = 1; i <= r1; i++)	
	{
		Y(i,1) = (*Xi)(i,1);
		Y(i,2) = (*Xi)(i,2);
	}
	
	Mat<> hah = inv(L)*Y;

	//h
	for(int i = 1; i <= (r1+3); i++)	h(i) = hah(i,1);
	for(int i = 1; i <= r1; i++)	h(r1+3+i) = (*xi)(i,1);	
	for(int i = 1; i <= (r1+3); i++)	h(p/2+i) = hah(i,2);
	for(int i = 1; i <= r1; i++)	h(p/2+r1+3+i) = (*xi)(i,2);

	//sig0 and v
	v.reset(n*r1+q);
	is_solved = true;
	Mat<> X2 = transform_points(*xi);		
	is_solved = false;
    sig0 = 0;
	
    for(int i = 1; i <= r1; i++)
		for(int j = 1 ; j <= n ; j++)
		{				
			v( (i-1)*n + j ) = X2(i,j)-(*Xi)(i,j);
			sig0 += ( v( (i-1)*n + j )*v( (i-1)*n + j ) );				
		}        
	sig0 = sqrt(sig0/r1);
    //sig0 and v
	
	nofc = 0;
	is_solved = true;  
}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"complicated :-)";		
		out<<endl<<"Computed parameters are:";        
        return out;
	}

protected:

void ApproxSolution(){}
void fill_matrixes() {}

public:
Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int en = x.rows();
		Mat<> Adx(en,3);		

		for(int i = 1; i<=en ; i++)
		{
			Adx(i,1) = 1;
			Adx(i,2) = x(i,1);
			Adx(i,3) = x(i,2);
		}		

		int nn = (p/2-3)/2;
		Mat<> X(nn,2);
		for(int i = 1; i <= nn; i++)
		{
			X(i,1) = h(nn+3+i);
			X(i,2) = h(p/2+nn+3+i);
		}

		Mat<> K(en,nn);		
		double rar;
		for(int i = 1; i <= en; i++)
			for(int j = 1; j <= nn; j++)
			{
				rar = sqrt( (X(j,1)-x(i,1))*(X(j,1)-x(i,1)) + (X(j,2)-x(i,2))*(X(j,2)-x(i,2)) );				
				if(rar != 0)	K(i,j) = rar*rar*log(rar*rar);
				else K(i,j) = 0;
			}

		/*
		//ukazka problemu
		Mat<> a(3,2),W(nn,2);
		for(int i = 1; i<= 3; i++)
		{
			a(i,1) = h(nn+i);
			a(i,2) = h(p/2+nn+i);
		}
		for(int i = 1; i <= nn; i++)
		{			
			W(i,1) = h(i);
			W(i,2) = h(p/2+i);
		}

		Mat<> transa = trans(a);
		Mat<> transAdx = trans(Adx);

		kout<<endl<<"trans(a): "<<trans(a);
		kout<<endl<<"trans(Adx): "<<trans(Adx);
		kout<<endl<<"trans(a)*trans(Adx): "<<trans(a)*trans(Adx);
		kout<<endl<<"a spravne transa*transAdx: "<<transa*transAdx;
		
        Mat<> Xtransfrormed = trans(transa*transAdx) + K * W;
		*/

		Mat<> at(2,3),W(nn,2);
		for(int i = 1; i<= 3; i++)
		{
			at(1,i) = h(nn+i);
			at(2,i) = h(p/2+nn+i);
		}
		for(int i = 1; i <= nn; i++)
		{			
			W(i,1) = h(i);
			W(i,2) = h(p/2+i);
		}

        Mat<> Xtransformed = trans(at*trans(Adx)) + K * W;
		
		X.reset(en,3);
		for(int j = 1;j<=en;j++)	
		{
			X(j,1) = Xtransformed(j,1);
			X(j,2) = Xtransformed(j,2);
			X(j,3) = x(j,3);
		}		
		return X;
	}	
	else
	{
		throw at_exception("thin_plate_spline_2d::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   
#endif