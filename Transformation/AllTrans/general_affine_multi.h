#ifndef all_tran_general_affine_multi_h_
#define all_tran_general_affine_multi_h_

#include <string>
#include "../gmatvec/matvec.h"
#include "../gmatvec/added_by_koska.h"

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class general_affine_linear : public all_tran_base  // multi dimensional "the only linear" transformation 
{
public:
general_affine_linear(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi, XXi.cols()-1 + (XXi.cols()-1)*(XXi.cols()-1) )
	{tt="general_affine_linear";}			

general_affine_linear(const Vec<> hh,int cn) : all_tran_base(hh,cn + cn * cn)  //cn control dimension because it is multidimensional
    {tt="general_affine_linear";n = cn;}

general_affine_linear(int cn) : all_tran_base(cn + cn * cn)
	{tt="general_affine_linear";n = cn;}

virtual ~general_affine_linear (){}
//virtual
ostream& report_constants(ostream& out) const {return out;}                      

protected:

void ApproxSolution()
{
	h.set_zero();	//not necessary because of linear form
}

void fill_matrixes() 
{
	//J
	for(int i = 0; i < r1; i++)
		for(int j = 1; j<=n; j++)
		{			
			J(i*n+j,j)= 1;
			for(int jj=1; jj<=n;jj++)	J(i*n+j,j*n+jj)= (*xi)(i+1,jj);				
		}		
	//X0T	
	Vec<> hhJ(p);
	for(int i = 0; i < r1; i++)
		for(int j=1; j<=n;j++)
		{			
			for(int ii = 1; ii<=p; ii++)	hhJ(ii) = J(i*n+j,ii);							
			X0T(i+1,j) = hhJ * h;
		}	
}

Mat<> transform_points(const Mat<>& x) //new
{
	if(is_solved)
	{		
		Vec<> tX0(n);
		Mat<> tR(n,n);
		for(int i = 1; i <= n; i++)
		{
			tX0(i) = h(i);
			for(int j = 1; j <= n; j++)	tR(i,j) = h(n*i+j);			
		}			
		int num = x.rows();
		Mat<> X(num,n+1);
		for(int j = 1;j<=num;j++)	X(j,n+1) = x(j,n+1);
		Vec<> tx(n),tX(n);
		for(int i = 1; i <= num; i++)
		{
			for(int j = 1; j<=n; j++)	tx(j) = x(i,j);
			tX = tX0 + tR * tx;
			for(int j = 1; j<=n; j++)	X(i,j) = tX(j);
		}		
		return X;
	}	
	else
	{
		throw at_exception("general_affine_linear::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}	
};


class general_affine_m : public all_tran_base  
{
public:
general_affine_m(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,2*(XXi.cols()-1)+(XXi.cols()-1)*(XXi.cols()-1))
        {tt="general_affine_m"; q=n; R.reset(n,n);}
		
general_affine_m(const Vec<> hh,int cn) : all_tran_base(hh,2*cn+cn*cn) //cn control dimension because it is multidimensional
        {tt="general_affine_m"; n=cn; q=n; R.reset(n,n);}

general_affine_m(int cn) : all_tran_base(2*cn+cn*cn)	//cn control dimension because it is multidimensional
		{tt="general_affine_m"; n=cn; q=n; R.reset(n,n);}


//because it is not only stand alone transformation,
//but also base class for affine_etc_3d and affine_etc_2d
general_affine_m(const Vec<> hh,int cn, int cp) : all_tran_base(hh,cp) //cn control dimension because it is multidimensional
        {tt="general_affine_m"; n=cn; R.reset(n,n);}		//cp control numbers of variables

general_affine_m(int cn,int cp) : all_tran_base(cp) //cn control dimension because it is multidimensional
        {tt="general_affine_m"; n=cn; R.reset(n,n);}


virtual ~general_affine_m (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = X0 + M * R * x";
        out<<endl<<"Computed parameters are (in order X0(1,dim), M(1,dim), R(dim,dim))";
        return out;}

protected:
Mat<> R;


void ApproxSolution()
{	
	if( p <= (r1*n+n) ) 
	{
		Vec<> hh;
		general_affine_linear app((*Xi),(*xi));
		//kout<<'\n'<<(*Xi)<<'\n'<<*xi<<endl;
		app.solve();
		hh = app.get_solution();

		//R
		for(int i = 1; i<=n; i++)
			for(int j = 1; j<=n; j++)	R(i,j) = hh( n+n*(i-1)+j );	
		//X0
		for(int i = 1; i<=n; i++)	h(i) = hh(i);

		Vec<> sum(n);
		sum.set_zero();
		for(int i = 1; i<=n; i++)
			for(int j = 1; j<=n; j++)	sum(i) += R(i,j)*R(i,j);	
		//M
		for(int i = 1; i<=n; i++)	{sum(i) = sqrt(sum(i));h(n+i) = sum(i);} 
		for(int i = 1; i<=n; i++)
			for(int j = 1; j<=n; j++)	R(i,j) = R(i,j)/sum(i);
		//R
		for(int i = 1; i<=n; i++)
			for(int j = 1; j<=n; j++)	h(2*n+n*(i-1)+j) = R(i,j);		
	}	
	else
	{
		//h.set_zero();
		if( n == 2 && r1 == 2 )
		{
			//ROTATION
			double bearing_x = bearing((*xi)(1,1),(*xi)(1,2),(*xi)(2,1),(*xi)(2,2));
			double bearing_X = bearing((*Xi)(1,1),(*Xi)(1,2),(*Xi)(2,1),(*Xi)(2,2));
			double omega = bearing_X - bearing_x;
			R(1,1) = cos(omega); R(1,2) = -sin(omega);
			R(2,1) = sin(omega); R(2,2) = cos(omega);
			//SCALE
			if( q == 5 )	{h(3) = 1;h(4) = 1;}
			else
			{
				double distance_x = distance((*xi)(1,1),(*xi)(1,2),(*xi)(2,1),(*xi)(2,2));
				double distance_X = distance((*Xi)(1,1),(*Xi)(1,2),(*Xi)(2,1),(*Xi)(2,2));
				h(3) = distance_X/distance_x;
				h(4) = h(3);
			}
		}		
		if( n == 3 && r1 == 3 )
		{
			//ROTATION
			Vec<> nx(3),ny(3),ny2(3),nz(3),nX(3),nY(3),nY2(3),nZ(3);
			double ee_norm,e_normx,e_normy,e_normX,e_normY;
			//normed vector 12
			//nx
			for(int i = 1; i<=3; i++)	nx(i) = (*xi)(2,i)-(*xi)(1,i);			
			e_normx = e_norm(nx);
			for(int i = 1; i<=3; i++)	nx(i) /= e_normx;			
			//nX
			for(int i = 1; i<=3; i++)	nX(i) = (*Xi)(2,i)-(*Xi)(1,i);			
			e_normX = e_norm(nX);
			for(int i = 1; i<=3; i++)	nX(i) /= e_normX;
			//normed vector y
			//ny
			for(int i = 1; i<=3; i++)	ny(i) = (*xi)(3,i)-(*xi)(1,i);	
			e_normy = e_norm(ny);
			double nxny = 0;
			for(int i = 1; i<=3; i++)	nxny += nx(i)*ny(i);
			for(int i = 1; i<=3; i++)	ny2(i) = nxny*nx(i);
			for(int i = 1; i<=3; i++)	ny(i) = ny(i)-ny2(i);
			ee_norm = e_norm(ny);
			for(int i = 1; i<=3; i++)	ny(i) /= ee_norm;
			//nY
			for(int i = 1; i<=3; i++)	nY(i) = (*Xi)(3,i)-(*Xi)(1,i);	
			e_normY = e_norm(nY);
			nxny = 0;
			for(int i = 1; i<=3; i++)	nxny += nX(i)*nY(i);
			for(int i = 1; i<=3; i++)	nY2(i) = nxny*nX(i);
			for(int i = 1; i<=3; i++)	nY(i) = nY(i)-nY2(i);
			ee_norm = e_norm(nY);
			for(int i = 1; i<=3; i++)	nY(i) /= ee_norm;
			//normed vector z
			//nz
			nz(1) = nx(2)*ny(3)-ny(2)*nx(3);
			nz(2) = ny(1)*nx(3)-nx(1)*ny(3);
			nz(3) = nx(1)*ny(2)-ny(1)*nx(2);
			//nZ
			nZ(1) = nX(2)*nY(3)-nY(2)*nX(3);
			nZ(2) = nY(1)*nX(3)-nX(1)*nY(3);
			nZ(3) = nX(1)*nY(2)-nY(1)*nX(2);
			//transformation matrixes
			Mat<> RxT(3,3);
			for(int i = 1; i<=3; i++)
			{
				RxT(1,i) = nx(i);
				RxT(2,i) = ny(i);
				RxT(3,i) = nz(i);
			}
			Mat<> RX(3,3);
			for(int i = 1; i<=3; i++)
			{
				RX(i,1) = nX(i);
				RX(i,2) = nY(i);
				RX(i,3) = nZ(i);
			}
			//transformation matrix R=RX*RxT
			R = RX*RxT;
			//SCALE
			if( q == 9 )	{h(4) = 1;h(5) = 1;h(6) = 1;}
			else
			{
				h(4) = (e_normX/e_normx + e_normY/e_normy)/2;
				h(5) = h(4);h(6) = h(4);
			}
		}
		else throw at_exception("general_affine_m::ApproxSolution","There is no enough points to compute transformation key");
				
		//for both situation 2D and 3D
		//TRANSLATIN X0
		for(int i = 1; i<=n; i++)
		{
			h(i) = (*Xi)(1,i);
			for(int j = 1; j<=n; j++)	h(i) -= h(n+i)*R(i,j)*(*xi)(1,j);
		}
		//R
		for(int i = 1; i<=n; i++)
			for(int j = 1; j<=n; j++)	h(2*n+n*(i-1)+j) = R(i,j);
	}		
}

void fill_matrixes_general_affine_m()
{
	//J	
	for(int i = 1; i<=n; i++)
		for(int j = 1; j<=n; j++)	R(i,j) = h( 2*n+n*(i-1)+j );

	for(int i = 0; i < r1; i++)
	{
		for(int j=1; j<=n;j++)
		{
            J(i*n+j,j)= 1;
			J(i*n+j,n+j) = 0;
			for(int jj=1; jj<=n;jj++)
			{
				J(i*n+j,n+j) += R(j,jj)*(*xi)(i+1,jj);				
				J(i*n+j,j*n+n+jj)= (*xi)(i+1,jj)*h(n+j);			
			}
		}
	}
	//X0T
	double hu;
	for(int i = 0; i < r1; i++)
	{
		for(int j=1; j<=n;j++)
		{
			hu = 0;
			for(int jj=1; jj<=n;jj++)	hu += R(j,jj)*(*xi)(i+1,jj);			
			X0T(i+1,j) = h(j) + h(n+j) * hu;
		}
	}
	//constraints
	//dX directly instead of X0T, dX = -X0T
	for(int i = 1; i <= n; i++)
	{
		b(i) = 0;
		for(int j=1; j<=n;j++)	b(i) -= R(i,j)*R(i,j); //dX(r1*n+i) -= R(j,i)*R(j,i); for columns
		b(i) += 1 ;
	}
	//J
	for(int i = 1; i <= n; i++)
		for(int j = 1; j <= n; j++)	B(n+i*n+j,i) = 2*R(i,j);
		//for(int j = 1; j <= n; j++)	J(r1*n+i,n+n*j+i) = 2*R(j,i); for columns	
}

void fill_matrixes()	{fill_matrixes_general_affine_m();}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		//init mat
		R.reset(n,n);
		for(int i = 1; i<=n; i++)
			for(int j = 1; j<=n; j++)	R(i,j) = h( 2*n+n*(i-1)+j );	
		//init mat

		int num = x.rows();
		Mat<> X(num,n+1);
		for(int j = 1;j<=num;j++)	X(j,n+1) = x(j,n+1);
		double hu;
		for(int i = 0; i < num; i++)
		{
			for(int j=1; j<=n;j++)
			{
				hu = 0;
				for(int jj=1; jj<=n;jj++)	hu += R(j,jj)*x(i+1,jj);			
				X(i+1,j) = h(j) + h(n+j) * hu;
			}
		}		
		return X;
	}	
	else
	{
		throw at_exception("general_affine_m::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
};

}   // namespace spat_fig
#endif