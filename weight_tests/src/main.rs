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
        if self.deck.len() == 0 {
            panic!("Cannot draw from empty deck");
        }

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
struct Item(&'static str);

impl Item {
    pub fn new(id: &'static str) -> Self {
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

            if attempts > 1_000 {
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

fn simulate_deck(mut deck: Deck<Item>, draw_iters: usize) {
    let mut stats = draw_stats(&mut deck, draw_iters)
        .into_iter()
        .collect::<Vec<_>>();
    stats.sort_unstable_by(|(_, count), (_, other_count)| other_count.cmp(count));
    let mut total = 0_f32;
    for (item, count) in stats {
        let frac = (count as f32) / draw_iters as f32;
        println!("{}: {}", item.0, frac);
        total += frac;
    }

    println!("Total: {total}");
}

fn main() {
    let mut deck = Deck::new(Vec::<Item>::new());

    // add weights to deck
    deck.insert_count(Item("ONITwitch.SpawnDupe"), 50);
    deck.insert_count(Item("ONITwitch.ElementGroupCommon"), 100);
    deck.insert_count(Item("ONITwitch.ElementGroupExotic"), 10);
    deck.insert_count(Item("ONITwitch.ElementGroupMetal"), 50);
    deck.insert_count(Item("ONITwitch.ElementGroupGas"), 50);
    deck.insert_count(Item("ONITwitch.ElementGroupLiquid"), 50);
    deck.insert_count(Item("ONITwitch.ElementGroupDeadly"), 10);
    deck.insert_count(Item("ONITwitch.AttributeAthleticsUp"), 20);
    deck.insert_count(Item("ONITwitch.AttributeAthleticsDown"), 20);
    deck.insert_count(Item("ONITwitch.AttributeConstructionUp"), 20);
    deck.insert_count(Item("ONITwitch.AttributeConstructionDown"), 20);
    deck.insert_count(Item("ONITwitch.AttributeExcavationUp"), 20);
    deck.insert_count(Item("ONITwitch.AttributeExcavationDown"), 20);
    deck.insert_count(Item("ONITwitch.AttributeStrengthUp"), 20);
    deck.insert_count(Item("ONITwitch.AttributeStrengthDown"), 20);
    deck.insert_count(Item("ONITwitch.BansheeWail"), 10);
    deck.insert_count(Item("ONITwitch.SnowBedrooms"), 20);
    deck.insert_count(Item("ONITwitch.SlimeBedrooms"), 20);
    deck.insert_count(Item("ONITwitch.FloodWater"), 30);
    deck.insert_count(Item("ONITwitch.FloodPollutedWater"), 30);
    deck.insert_count(Item("ONITwitch.FloodEthanol"), 10);
    deck.insert_count(Item("ONITwitch.FloodOil"), 10);
    deck.insert_count(Item("ONITwitch.FloodLava"), 2);
    deck.insert_count(Item("ONITwitch.FloodGold"), 2);
    deck.insert_count(Item("ONITwitch.FloodNuclearWaste"), 2);
    deck.insert_count(Item("ONITwitch.IceAge"), 1);
    deck.insert_count(Item("ONITwitch.Pee"), 30);
    deck.insert_count(Item("ONITwitch.Kill"), 1);
    deck.insert_count(Item("ONITwitch.TileTempDown"), 10);
    deck.insert_count(Item("ONITwitch.TileTempUp"), 10);
    deck.insert_count(Item("ONITwitch.PartyTime"), 30);
    deck.insert_count(Item("ONITwitch.PoisonDupes"), 20);
    deck.insert_count(Item("ONITwitch.Poopsplosion"), 50);
    deck.insert_count(Item("ONITwitch.RainPrefabGold"), 30);
    deck.insert_count(Item("ONITwitch.RainPrefabMorb"), 30);
    deck.insert_count(Item("ONITwitch.RainPrefabDiamond"), 10);
    deck.insert_count(Item("ONITwitch.RainPrefabSlickster"), 20);
    deck.insert_count(Item("ONITwitch.RainPrefabPacu"), 20);
    deck.insert_count(Item("ONITwitch.RainPrefabBee"), 10);
    deck.insert_count(Item("ONITwitch.ReduceOxygen"), 10);
    deck.insert_count(Item("ONITwitch.SkillPoints"), 20);
    deck.insert_count(Item("ONITwitch.SleepyDupes"), 20);
    deck.insert_count(Item("ONITwitch.SnazzySuit"), 20);
    deck.insert_count(Item("ONITwitch.SpawnGlitterPuft"), 20);
    deck.insert_count(Item("ONITwitch.SpawnVacillatorCharge"), 5);
    deck.insert_count(Item("ONITwitch.SpawnAtmoSuit"), 5);
    deck.insert_count(Item("ONITwitch.SpawnCrab"), 10);
    deck.insert_count(Item("ONITwitch.SpawnMooComet"), 10);
    deck.insert_count(Item("ONITwitch.StressAdd"), 20);
    deck.insert_count(Item("ONITwitch.StressRemove"), 20);
    deck.insert_count(Item("ONITwitch.Surprise"), 50);
    deck.insert_count(Item("ONITwitch.Uninsulate"), 5);
    deck.insert_count(Item("ONITwitch.ResearchTech"), 10);
    deck.insert_count(Item("ONITwitch.Eclipse"), 10);
    deck.insert_count(Item("ONITwitch.PocketDimension"), 20);
    deck.insert_count(Item("ONITwitch.MeltMagma"), 20000);

    // simulate and report drawn chances
    const DRAW_ITERATIONS: usize = 1_000_000;
    simulate_deck(deck, DRAW_ITERATIONS);
}
