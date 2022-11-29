use std::collections::HashMap;
use std::{array, iter};

use rand::seq::SliceRandom;
use rand::thread_rng;

#[derive(Debug)]
pub struct Deck<T: Clone> {
    deck: Vec<T>,
    head_idx: usize,
}

impl<T: Clone> Deck<T> {
    pub fn new(data: Vec<T>) -> Self {
        Self {
            deck: data,
            head_idx: 0,
        }
    }

    pub fn draw(&mut self) -> T {
        if self.head_idx == self.deck.len() {
            self.deck.shuffle(&mut thread_rng());
            self.head_idx = 0;
        }

        let ret = self.deck[self.head_idx].clone();
        self.head_idx += 1;
        ret
    }

    pub fn insert_count(&mut self, item: T, count: usize)
    where
        T: Clone,
    {
        self.deck.extend(iter::repeat(item).take(count));
        self.deck.shuffle(&mut thread_rng());
        self.head_idx = 0;
    }

    pub fn move_last_drawn_to_end(&mut self) {
        if self.head_idx != 0 {
            let item = self.deck.swap_remove(self.head_idx - 1);
            self.deck.push(item);
        }
    }

    pub fn count(&self) -> usize {
        self.deck.len()
    }
}

#[derive(Clone, Copy, Debug, PartialEq, Eq, PartialOrd, Ord, Hash)]
struct Item(usize);

impl Item {
    pub fn new(id: usize) -> Self {
        Self(id)
    }
}

// repeatedly draw from the deck several times, keeping track of how often things appear
fn draw_stats<T: PartialEq + Eq + core::hash::Hash + Clone + core::fmt::Debug>(
    deck: &mut Deck<T>,
    draw_iterations: usize,
) -> HashMap<T, i32> {
    const DRAW_SIZE: usize = 3;

    let mut count_map = HashMap::new();

    for _ in 0..draw_iterations {
        let mut results = array::from_fn::<_, DRAW_SIZE, _>(|_| None);
        let mut drawn = 0;
        let mut attempts = 0;
        while drawn < DRAW_SIZE {
            let entry = Some(deck.draw());
            if !results.contains(&entry) {
                results[drawn] = entry;
                drawn += 1;
            }

            attempts += 1;

            if attempts > 10_000 {
                panic!("Unable to draw enough entries");
            }
        }

        for e in results {
            count_map
                .entry(e.unwrap())
                .and_modify(|val| *val += 1)
                .or_insert(1);
        }
    }

    count_map
}

fn main() {
    let mut deck = Deck::new(Vec::<Item>::new());
    deck.insert_count(Item::new(1), 499);
    deck.insert_count(Item::new(2), 499);
    deck.insert_count(Item::new(3), 1);
    deck.insert_count(Item::new(4), 1);
    deck.insert_count(Item::new(5), 10);
    deck.insert_count(Item::new(6), 10);
    deck.insert_count(Item::new(7), 10);
    deck.insert_count(Item::new(8), 10);
    deck.insert_count(Item::new(9), 10);
    deck.insert_count(Item::new(10), 10);
    deck.insert_count(Item::new(11), 10);
    deck.insert_count(Item::new(12), 10);
    deck.insert_count(Item::new(13), 10);
    deck.insert_count(Item::new(14), 10);
    deck.insert_count(Item::new(15), 10);
    deck.insert_count(Item::new(16), 10);

    const DRAW_ITERATIONS: usize = 100_000;
    let mut stats = draw_stats(&mut deck, DRAW_ITERATIONS)
        .into_iter()
        .collect::<Vec<_>>();
    stats.sort_unstable();
    for (item, count) in stats {
        println!("{item:?}: {}", (count as f32) / DRAW_ITERATIONS as f32);
    }
}
