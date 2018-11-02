using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIbrain : MonoBehaviour {

	public List<HexUnit> Units;

    public HexGrid grid;

    public Canvas Win;

    public void Activate() {
        if(Units.Count<=0) { 
            //Game Over Player Won!
            Win.gameObject.SetActive(true);
            return;
        }

        foreach (var U in Units) { 
            MakeValidMove(U);   
        }
        foreach (var F in grid.Forts) { 
            F.SpawnUnit();    
        }

    }

    bool TentaAtacar(HexUnit U){ 
        for (HexDirection D = HexDirection.NE; D <= HexDirection.NW; D++) {
                if (U.Location.GetNeighbor(D))
                    if (U.Location.GetNeighbor(D).Unit)
                        if(U.Location.GetNeighbor(D).Unit.Faccao!="Barbaros" && U.Location.GetNeighbor(D).Unit.Faccao!="Minor") { //se nao for menor ou barbaro
				            U.Attack(U.Location.GetNeighbor(D).Unit);
                            return true;
                        }
        }
        return false;
    }

    void MakeValidMove(HexUnit U) { 

        int[] dir= new int[] {-1,-1}; //vetor para evitar que a unidade retorne durante o movimento.
        
        bool valid = false;
        HexCell aux, final = U.Location; //aux faz varredura, final salva a celula que a unidade para.

        if(TentaAtacar(U)) //se atacar nao precisa mais mover.
            return;

        bool canMove=false; //verifica se existe movimento possivel.
        for (HexDirection D = HexDirection.NE; D <= HexDirection.NW; D++) {
            grid.FindPath(U.Location,U.Location.GetNeighbor(D), U);
            if (grid.HasPath)
                canMove=true;
            grid.ClearPath();
        }
        if(!canMove)
            return;

        while(!valid) {

            //sorteia uma direcao e verifica se ela nao eh o retorno de um movimento ja realizado.
            HexDirection d = (HexDirection)Random.Range(0,6);
            if(dir[1]!=-1 && (HexDirection)dir[1] == d)
                continue;
            if(dir[0]!=-1 && (HexDirection)dir[0] == d)
                continue;
            
            //uma vez que o sorteio resulte em uma direcao valida, verifica se a nova celula eh valida: esta ao lado de um inimigo ou nao possui nenhum aliado em volta.
            aux=final.GetNeighbor(d);
            if(!aux)
                continue;

            for (HexDirection D = HexDirection.NE; D <= HexDirection.NW; D++) {
                if (aux.GetNeighbor(D))
                    if (aux.GetNeighbor(D).Unit)
                        if(aux.GetNeighbor(D).Unit.Faccao!="Barbaros" && aux.GetNeighbor(D).Unit.Faccao!="Minor"){ 
				            valid = true; //tem unidade atacavel
                        }
            }
        /*
            bool cont=false;
            for (HexDirection D = HexDirection.NE; D <= HexDirection.NW; D++) {
                if (aux.GetNeighbor(D))
                    if (aux.GetNeighbor(D).Unit)
                        if ((aux.GetNeighbor(D).Unit.Faccao=="Barbaros" || aux.GetNeighbor(D).Unit.Faccao=="Minor") && !valid) {
                            // se for faccao da IA e nao tem unidade atacavel em volta, melhor nao seguir na direcao;
				            cont=true;
                        }
            }
            if(cont)
                continue;
                */
                grid.FindPath(final,final.GetNeighbor(d), U);
                    if (grid.HasPath) {

			            final = final.GetNeighbor(d);
			            grid.ClearPath();

                        if(dir[0]==-1)
                            dir[0]=(int)d.Opposite();
                        else if(dir[1]==-1)
                            dir[1]=(int)d.Opposite();
                        else
                            valid = true;
		            }
                    else
                        valid = false;
                
                
        }
        grid.FindPath(U.Location,final,U);
            if(grid.HasPath) { 
                U.Travel(grid.GetPath());
                grid.ClearPath();
            }
        TentaAtacar(U);
    }
}
