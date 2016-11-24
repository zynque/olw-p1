#light

module FSOLP.Tree

//-----------------------------------------------------------------------------

type Node<'d> = { Children: Node<'d> list; Data: 'd}

let rec disp n =
    System.Console.Write("(" + n.Data.ToString())
    dispChildren n.Children
    System.Console.Write(")")

and dispChildren cs =
    match cs with
    | [] -> ()
    | c :: cs -> disp c; dispChildren cs

let insertAfter tree node newNode =
    let children =
        [
          for item in tree.Children do
            yield item
            if item = node then yield newNode
        ]
    {Children = children; Data = tree.Data}

let insertBefore tree node newNode =
    let children =
        [
          for item in tree.Children do
            if item = node then yield newNode
            yield item
        ]
    {Children = children; Data = tree.Data}

let insertAt tree index newNode =
    let i = ref 0
    let children =
        [
            for item in tree.Children do
              if !i = index then
                  yield newNode
              i := !i + 1
              yield item
            if !i = index then
                yield newNode
        ]
    {Children = children; Data = tree.Data}

let remove tree node =
    let children =
        [
          for item in tree.Children do
            if item <> node then
              yield item
        ]
    {Children = children; Data = tree.Data}

let replace tree node newNode =
    let children =
        [
          for item in tree.Children do
            if item = node then
              yield newNode
            else
              yield item
        ]
    {Children = children; Data = tree.Data}

let rec applyDelta stack (n, np) =
    match stack with
    | [] -> failwith "Empty stack"
    | t::[] -> replace t n np
    | t::ts -> applyDelta ts (t, (replace t n np))
